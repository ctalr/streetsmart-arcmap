/*
 * Integration in ArcMap for Cycloramas
 * Copyright (c) 2019, CycloMedia, All rights reserved.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.
 */

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using StreetSmart.Common.Factories;
using StreetSmart.Common.Interfaces.Data;
using StreetSmart.Common.Interfaces.GeoJson;
using StreetSmart.Common.Interfaces.SLD;
using StreetSmartArcMap.AddIns;
using StreetSmartArcMap.Client;
using StreetSmartArcMap.Logic;
using StreetSmartArcMap.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IFeature = ESRI.ArcGIS.Geodatabase.IFeature;
using IPoint = ESRI.ArcGIS.Geometry.IPoint;

namespace StreetSmartArcMap.Layers
{
    #region delegates

    // ===========================================================================
    // Delegates
    // ===========================================================================
    public delegate void VectorLayerAddDelegate(VectorLayer layer);

    public delegate void VectorLayerChangedDelegate(VectorLayer layer);

    public delegate void VectorLayerRemoveDelegate(VectorLayer layer);

    public delegate void FeatureStartEditDelegate(IList<ESRI.ArcGIS.Geometry.IGeometry> geometries);

    public delegate void FeatureUpdateEditDelegate(ESRI.ArcGIS.Geodatabase.IFeature feature);

    public delegate void FeatureDeleteDelegate(ESRI.ArcGIS.Geodatabase.IFeature feature);

    public delegate void StopEditDelegate();

    public delegate void StartMeasurementDelegate(TypeOfLayer typeOfLayer);

    public delegate void SketchCreateDelegate(IEditSketch3 sketch);

    public delegate void SketchModifiedDelegate(ESRI.ArcGIS.Geometry.IGeometry geometry);

    public delegate void SketchFinishedDelegate();

    #endregion delegates

    public class VectorLayer
    {
        #region constants

        // =========================================================================
        // Constants
        // =========================================================================
        private const string WfsHeader =
          "<wfs:FeatureCollection xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:wfs=\"http://www.opengis.net/wfs\" xmlns:gml=\"http://www.opengis.net/gml\">";

        private const string WfsFinished = "</wfs:FeatureCollection>";

        #endregion constants

        #region members

        // =========================================================================
        // Members
        // =========================================================================
        public static event VectorLayerAddDelegate LayerAddEvent;

        public static event VectorLayerRemoveDelegate LayerRemoveEvent;

        public static event VectorLayerChangedDelegate LayerChangedEvent;

        public static event FeatureStartEditDelegate FeatureStartEditEvent;

        public static event FeatureUpdateEditDelegate FeatureUpdateEditEvent;

        public static event FeatureDeleteDelegate FeatureDeleteEvent;

        public static event StopEditDelegate StopEditEvent;

        public static event StartMeasurementDelegate StartMeasurementEvent;

        public static event SketchCreateDelegate SketchCreateEvent;

        public static event SketchModifiedDelegate SketchModifiedEvent;

        public static event SketchFinishedDelegate SketchFinishedEvent;

        public IStyledLayerDescriptor Sld { get; private set; }

        private static string LastEditedPointFeature = string.Empty;
        private static int LastEditedObject = -1;

        private static IList<ESRI.ArcGIS.Geodatabase.IFeature> _editFeatures;
        private static IList<VectorLayer> _layers;
        private static System.Windows.Forms.Timer _editToolCheckTimer;
        private static ICommandItem _beforeTool;
        private static readonly LogClient LogClient;
        private static readonly object LockObject;
        private static bool _doSelection;
        private static Timer _nextSelectionTimer;
        private static bool _fromSelection;

        private IFeatureClass _featureClass;
        private ILayer _layer;
        private bool _isVisibleInStreetSmart;

        private static IEnvelope _oldEnvelope = null;
        private static IEnvelope _newEnvelope = null;

        public IOverlay Overlay;

        private static string CurrentMeasurementID = "";

        private Configuration.Configuration Config => Configuration.Configuration.Instance;

        #endregion members

        #region constructor

        // =========================================================================
        // Constructor
        // =========================================================================
        static VectorLayer()
        {
            _editToolCheckTimer = null;
            _beforeTool = null;
            LogClient = new LogClient(typeof(VectorLayer));
            LockObject = new object();
            _doSelection = true;
            _fromSelection = false;
        }

        #endregion constructor

        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        private IFeatureCollection _contents;

        public bool ContentsChanged { get; private set; }
        public string Name => _layer != null ? _layer.Name : string.Empty;
        public bool IsVisible => _layer != null && _layer.Visible;

        public TypeOfLayer TypeOfLayer => GetTypeOfLayer(_featureClass.ShapeType);
        public ISpatialReference SpatialReference => GeometryDef.SpatialReference;
        public static IList<ESRI.ArcGIS.Geodatabase.IFeature> EditFeatures => _editFeatures ?? (_editFeatures = new List<ESRI.ArcGIS.Geodatabase.IFeature>());
        public static IList<VectorLayer> Layers => _layers ?? (_layers = DetectVectorLayers(false));
        public static Dictionary<VectorLayer, ESRI.ArcGIS.Geodatabase.IFeature> MeasurementsList = new Dictionary<VectorLayer, ESRI.ArcGIS.Geodatabase.IFeature>();

        public bool IsVisibleInStreetSmart
        {
            get
            {
                return (_isVisibleInStreetSmart && IsVisible);
            }
            set
            {
                if (IsVisibleInStreetSmart != value)
                {
                    _isVisibleInStreetSmart = value;

                    SetVisibility(value);
                }
            }
        }

        public IGeometryDef GeometryDef
        {
            get
            {
                IFields fields = _featureClass.Fields;
                int fieldId = fields.FindField(_featureClass.ShapeFieldName);
                IField field = fields.get_Field(fieldId);

                return field.GeometryDef;
            }
        }

        public bool HasZ
        {
            get
            {
                Configuration.Configuration config = Configuration.Configuration.Instance;
                SpatialReference spatRel = config.SpatialReference;
                ISpatialReference gsSpatialReference = spatRel?.SpatialRef ?? ArcUtils.SpatialReference;

                bool zCoord = gsSpatialReference.ZCoordinateUnit != null;
                bool sameFactoryCode = SpatialReference.FactoryCode == gsSpatialReference.FactoryCode;

                return (spatRel == null || spatRel.CanMeasuring) && GeometryDef.HasZ;
            }
        }

        #endregion properties

        #region functions (static)

        // =========================================================================
        // Functions (Static)
        // =========================================================================

        public static TypeOfLayer GetTypeOfLayer(esriGeometryType type)
        {
            switch (type)
            {
                case esriGeometryType.esriGeometryPoint:
                    return TypeOfLayer.Point;

                case esriGeometryType.esriGeometryLine:
                    return TypeOfLayer.Line;

                case esriGeometryType.esriGeometryPolyline:
                    return TypeOfLayer.Line;

                case esriGeometryType.esriGeometryPolygon:
                    return TypeOfLayer.Polygon;

                default:
                    return TypeOfLayer.None;
            }
        }

        public static VectorLayer GetLayer(ILayer layer)
        {
            if (layer == null)
                return null;

            return Layers.Aggregate<VectorLayer, VectorLayer>(null,
                                                              (current, layerCheck) =>
                                                              (layerCheck._layer?.Name == layer?.Name) ? layerCheck : current);
        }

        public static VectorLayer GetLayer(IFeatureClass featureClass)
        {
            return Layers.Aggregate<VectorLayer, VectorLayer>(null,
                                                              (current, layerCheck) =>
                                                              (layerCheck._featureClass.FeatureClassID == featureClass.FeatureClassID)
                                                                ? layerCheck
                                                                : current);
        }

        public static VectorLayer GetLayer(ESRI.ArcGIS.Geodatabase.IFeature feature)
        {
            var featureClass = feature?.Class as IFeatureClass;

            if (featureClass != null)
                return GetLayer(featureClass);
            else
                return null;
        }

        public static IList<VectorLayer> DetectVectorLayers(bool initEvents)
        {
            _layers = new List<VectorLayer>();
            IMap map = ArcUtils.Map;

            if (map != null)
            {
                var layers = map.get_Layers();
                ILayer layer;

                while ((layer = layers.Next()) != null)
                {
                    AvItemAdded(layer);
                }
            }

            var editor = ArcUtils.Editor;

            if (editor.EditState != esriEditState.esriStateNotEditing)
            {
                //OnSelectionChanged();
            }

            if (initEvents)
            {
                AddEvents();
                var docEvents = ArcUtils.MxDocumentEvents;

                if (docEvents != null)
                {
                    docEvents.OpenDocument += OpenDocument;
                    docEvents.CloseDocument += CloseDocument;
                }
            }

            return _layers;
        }

        private static void AddEvents()
        {
            var avEvents = ArcUtils.ActiveViewEvents;
            var editEvents = ArcUtils.EditorEvents;
            var editEvents5 = ArcUtils.EditorEvents5;

            if (avEvents != null)
            {
                avEvents.ItemAdded += AvItemAdded;
                avEvents.ItemDeleted += AvItemDeleted;
                avEvents.ContentsChanged += AvContentChanged;
                avEvents.ViewRefreshed += AvViewRefreshed;
                //TODO: STREET-2002
                avEvents.AfterDraw += AvEvents_AfterDraw;
            }

            if (editEvents != null)
            {
                editEvents.OnStartEditing += OnStartEditing;
                editEvents.OnChangeFeature += OnChangeFeature;
                editEvents.OnSelectionChanged += OnSelectionChanged;
                editEvents.OnStopEditing += OnStopEditing;
                editEvents.OnDeleteFeature += OnDeleteFeature;
                editEvents.OnSketchModified += OnSketchModified;
                editEvents.OnSketchFinished += OnSketchFinished;
            }
        }

        private static List<ESRI.ArcGIS.Geometry.IPoint> GetGeometryPoints(ESRI.ArcGIS.Geometry.IGeometry geometry)
        {
            var points = new List<ESRI.ArcGIS.Geometry.IPoint>();

            var type = VectorLayer.GetTypeOfLayer(geometry.GeometryType);

            switch (type)
            {
                case TypeOfLayer.Point:
                    var point = geometry as ESRI.ArcGIS.Geometry.IPoint;

                    if (point != null)
                    {
                        points.Add(point);
                    }

                    break;

                case TypeOfLayer.Line:
                    var polyline = geometry as Polyline;

                    if (polyline != null)
                    {
                        for (int i = 0; i < polyline.PointCount; i++)
                        {
                            points.Add(polyline.Point[i]);
                        }
                    }

                    break;

                case TypeOfLayer.Polygon:
                    var polygon = geometry as Polygon;

                    if (polygon != null)
                    {
                        for (int i = 0; i < polygon.PointCount; i++)
                        {
                            points.Add(polygon.Point[i]);
                        }
                    }

                    break;

                default:
                    break;
            }

            return points;
        }

        private static double GetLabelOffset(IDisplay display)
        {
            IDisplayTransformation dispTrans = display.DisplayTransformation;

            double distance = dispTrans.FromPoints(12);

            return (distance * 3) / 4;
        }

        private static ESRI.ArcGIS.Geometry.IPolygon GetLabelBox(IDisplay display, ESRI.ArcGIS.Geometry.IPoint point)
        {
            IDisplayTransformation dispTrans = display.DisplayTransformation;

            var polygon = new PolygonClass();

            //TODO: STREET-2002
            polygon.AddPoint(new PointClass { X = point.X - dispTrans.FromPoints(6), Y = point.Y - dispTrans.FromPoints(6) });
            polygon.AddPoint(new PointClass { X = point.X + dispTrans.FromPoints(6), Y = point.Y - dispTrans.FromPoints(6) });
            polygon.AddPoint(new PointClass { X = point.X + dispTrans.FromPoints(6), Y = point.Y + dispTrans.FromPoints(6) });
            polygon.AddPoint(new PointClass { X = point.X - dispTrans.FromPoints(6), Y = point.Y + dispTrans.FromPoints(6) });

            polygon.Close();

            return polygon;
        }

        private static void AvEvents_AfterDraw(IDisplay display, esriViewDrawPhase phase)
        {
            if (phase == esriViewDrawPhase.esriViewForeground)
            {
                var sketch = ArcUtils.Editor as IEditSketch3;

                if (sketch != null && sketch.Geometry != null)
                {
                    display.StartDrawing(display.hDC, (short) esriScreenCache.esriNoScreenCache);

                    var fontDisp = new stdole.StdFontClass
                    {
                        Bold = true,
                        Name = "Arial",
                        Italic = false,
                        Underline = false,
                        Size = 8
                    };

                    var offset = GetLabelOffset(display);

                    RgbColor white = new RgbColorClass {Red = 255, Green = 255, Blue = 255};
                    RgbColor black = new RgbColorClass {Red = 0, Green = 0, Blue = 0};
                    ISymbol textSymbol = new TextSymbolClass {Font = fontDisp as stdole.IFontDisp, Color = black,};
                    display.SetSymbol(textSymbol);

                    ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass {Color = white};
                    ISymbol boxSymbol = simpleFillSymbol as ISymbol;
                    boxSymbol.ROP2 = esriRasterOpCode.esriROPWhite;

                    var points = GetGeometryPoints(sketch.Geometry);
                    if (sketch.GeometryType != esriGeometryType.esriGeometryPoint
                    ) // a point always gives an invalid geometry in this scenario...
                    {
                        for (int i = 0; i < points.Count; i++)
                        {
                            if (sketch.GeometryType == esriGeometryType.esriGeometryPolygon && i == points.Count - 1)
                                break; // a polygon always has the starting/end point twice, so skip the end point label for polygons.

                            var point = points[i];

                            if (!point.IsEmpty)
                            {
                                var originPoint = new PointClass {X = point.X + offset, Y = point.Y + offset};
                                var labelPoint = new PointClass {X = point.X + offset, Y = point.Y + offset / 2};
                                var labelText = (i + 1).ToString();

                                display.SetSymbol(boxSymbol);
                                display.DrawPolygon(GetLabelBox(display, originPoint));

                                display.SetSymbol(textSymbol);
                                display.DrawText(labelPoint, labelText);
                            }
                        }
                    }

                    int numberOfPoints = StreetSmartApiWrapper.Instance.GetNumberOfPoints();
                    const double distLine = 30.0;

                    for (int i = 0; i < numberOfPoints; i++)
                    {
                        var observations = StreetSmartApiWrapper.Instance.GetObservations(i);

                        if (observations != null)
                        {
                            foreach (var observation in observations)
                            {
                                var obsPanorama = observation as IResultDirectionPanorama;

                                if (obsPanorama != null)
                                {
                                    double x = obsPanorama.Position?.X ?? 0.0;
                                    double y = obsPanorama.Position?.Y ?? 0.0;
                                    double xDir = obsPanorama.Direction?.X ?? 0.0;
                                    double yDir = obsPanorama.Direction?.Y ?? 0.0;

                                    RgbColor gray = new RgbColorClass
                                        {Red = Color.Gray.R, Green = Color.Gray.G, Blue = Color.Gray.B};
                                    ISymbol lineSymbol = new SimpleLineSymbolClass {Color = gray, Width = 1.25};
                                    display.SetSymbol(lineSymbol);

                                    var polylineClass = new PolylineClass();
                                    polylineClass.AddPoint(
                                        new PointClass {X = x + xDir * distLine, Y = y + yDir * distLine});
                                    polylineClass.AddPoint(new PointClass {X = x, Y = y});
                                    display.DrawPolyline(polylineClass);
                                }
                            }
                        }
                    }

                    display.FinishDrawing();
                }
            }
        }

        private static bool CheckEditTask()
        {
            bool result = false;
            IEditor3 editor = ArcUtils.Editor;

            if (editor != null)
            {
                IEditTask task = editor.CurrentTask;

                if (task != null)
                {
                    var uniqueName = task as IEditTaskName;

                    if (uniqueName != null)
                    {
                        string name = uniqueName.UniqueName;
                        result = (name == "GarciaUI_CreateNewFeatureTask") || (name == "GarciaUI_ReshapeFeatureTask") || (name == "GarciaUI_ModifyFeatureTask");
                    }
                }
            }

            return result;
        }

        public static void UpdateEditGeometry(IEditor3 editor, ICommand command)
        {
            var sketch = editor as IEditSketch3;

            if (sketch != null)
            {
                var geometry = sketch.Geometry;

                if (!(command is IEditTool) && editor.EditState != esriEditState.esriStateNotEditing && StartMeasurementEvent != null)
                {
                    var typeOfLayer = GetTypeOfLayer(geometry.GeometryType);

                    StartMeasurementEvent(typeOfLayer);
                }
            }
        }

        private static ESRI.ArcGIS.Geometry.Point ConvertToPoint(ICoordinate coord, bool withZ)
        {
            if (coord.X.HasValue && coord.Y.HasValue)
            {
                var newEditPoint = new ESRI.ArcGIS.Geometry.Point();
                newEditPoint.X = coord.X.Value;
                newEditPoint.Y = coord.Y.Value;
                if (withZ)
                {
                    (newEditPoint as IZAware).ZAware = true;
                    newEditPoint.Z = coord.Z.HasValue ? coord.Z.Value : 0;
                }

                return newEditPoint;
            }
            return new ESRI.ArcGIS.Geometry.Point();
        }

        private static Polyline ConvertToPolyline(List<ICoordinate> coords, bool withZ)
        {
            Polyline polyline = new Polyline();
            ((PolylineClass)(polyline)).ZAware = withZ;

            foreach (var coord in coords)
            {
                var point = ConvertToPoint(coord, withZ);

                polyline.AddPoint(point);
            }

            return polyline;
        }

        public static void FinishMeasurement()
        {
            var sketch = ArcUtils.Editor as IEditSketch3;

            if (sketch != null && sketch.Geometry != null && !sketch.Geometry.IsEmpty)
                sketch.FinishSketch();
        }

        public static bool IsNewMeasurement(IFeatureCollection features)
        {
            if (features != null && features.Features.Count > 0)
            {
                var id = features.Features.First().Properties.Where(p => p.Key == "Id").Select(p => p.Value.ToString()).FirstOrDefault();

                if (CurrentMeasurementID != id)
                {
                    CurrentMeasurementID = id;

                    return true;
                }
            }

            return false;
        }

        public static void CreateMeasurement(IFeatureCollection features)
        {
            var activeView = ArcUtils.ActiveView;
            var display = activeView.ScreenDisplay;
            var editor = ArcUtils.Editor as IEditLayers;

            var layer = VectorLayer.GetLayer(editor.CurrentLayer);
            if (layer == null)
            {
                var selection = (IEnumFeature)ArcUtils.ActiveView.FocusMap.FeatureSelection;
                var f = selection.Next();
                if (f != null)
                {
                    var l = VectorLayer.GetLayer(f);
                    if (l != null)
                    {
                        editor.SetCurrentLayer((IFeatureLayer)l._layer, 0);
                        layer = l;
                    }
                }
                else
                {
                    return;
                }
            }

            var isNew = IsNewMeasurement(features);

            //Type is unknown when measurement is closed in Street Smart or a new one is started.
            if (features.Type == FeatureType.Unknown)
            {
                var sketch = editor as IEditSketch3;
                sketch.Geometry = null;
                sketch.RefreshSketch();
                OnLayerChanged(layer);
                ArcUtils.ActiveView.Refresh();

                ArcUtils.Editor.StopEditing(true);
            }

            if (!StreetSmartApiWrapper.Instance.BusyForMeasurement)
            {
                StreetSmartApiWrapper.Instance.BusyForMeasurement = true;

                foreach (var feature in features.Features)
                {
                    switch (feature.Geometry.Type)
                    {
                        case GeometryType.Point: // always add a new point
                            var coord = feature.Geometry as ICoordinate;
                            var point = ConvertToPoint(coord, layer.HasZ);

                            if (point != null && !point.IsEmpty)
                            {
                                IFeature newEditFeature;

                                if (LastEditedPointFeature != ((IMeasurementProperties) feature.Properties).Id)
                                {
                                    newEditFeature = layer._featureClass.CreateFeature();
                                }
                                else
                                {
                                    IEnumFeature editSelection = ArcUtils.Editor?.EditSelection;
                                    editSelection?.Reset();
                                    newEditFeature = editSelection.Next();
                                }

                                if (newEditFeature == null)
                                {
                                    if (LastEditedObject != -1)
                                    {
                                        try
                                        {
                                            newEditFeature = layer._featureClass.GetFeature(LastEditedObject);
                                        }
                                        catch (Exception)
                                        {
                                            newEditFeature = layer._featureClass.CreateFeature();
                                        }
                                    }
                                    else
                                    {
                                        newEditFeature = layer._featureClass.CreateFeature();
                                    }
                                }

                                if (newEditFeature != null)
                                {
                                    newEditFeature.Shape = point;
                                    newEditFeature.Store();

                                    LastEditedObject = newEditFeature.OID;
                                    OnLayerChanged(layer);
                                    ArcUtils.ActiveView.Refresh();
                                }
                            }
                            else if (LastEditedPointFeature != ((IMeasurementProperties)feature.Properties).Id)
                            {
                                LastEditedObject = -1;
                            }
                            else if (LastEditedObject != -1 && layer.TypeOfLayer == TypeOfLayer.Point)
                            {
                                var deleteFeature = layer._featureClass.GetFeature(LastEditedObject);
                                deleteFeature.Delete();
                                LastEditedObject = -1;
                            }

                            LastEditedPointFeature = ((IMeasurementProperties)feature.Properties).Id;
                            break;

                        case GeometryType.LineString:
                            var coords = feature.Geometry as List<ICoordinate>;

                            if (coords != null)
                            {
                                var sketch = editor as IEditSketch3;
                                IEnumFeature editSelection = ArcUtils.Editor?.EditSelection;
                                editSelection?.Reset();
                                var newEditFeature = editSelection.Next();

                                if (isNew)
                                {
                                    if (sketch != null && sketch.Geometry != null && !sketch.Geometry.IsEmpty)
                                    {
                                        // New measurement from Street Smart
                                        if ((sketch.Geometry as Polyline).PointCount > 1)
                                        {
                                            var geometry = sketch.Geometry;

                                            try
                                            {
                                                ((PolylineClass) geometry).ZAware = layer.HasZ;

                                                if (!_fromSelection)
                                                {
                                                    _fromSelection = false;
                                                    newEditFeature = layer._featureClass.CreateFeature();
                                                    newEditFeature.Shape = geometry;
                                                    newEditFeature.Store();
                                                    OnLayerChanged(layer);
                                                    sketch.Geometry = null;
                                                    sketch.RefreshSketch();
                                                    ArcUtils.ActiveView.Refresh();
                                                }
                                                else if (LastEditedObject != -1 && LastEditedObject == (newEditFeature?.OID ?? -1))
                                                {
                                                    _fromSelection = false;
                                                    newEditFeature.Shape = geometry;
                                                    newEditFeature.Store();
                                                    OnLayerChanged(layer);
                                                    sketch.Geometry = null;
                                                    sketch.RefreshSketch();
                                                    ArcUtils.ActiveView.Refresh();
                                                }
                                                else if (newEditFeature == null && LastEditedObject == -1)
                                                {
                                                    _fromSelection = false;
                                                    newEditFeature = layer._featureClass.CreateFeature();
                                                    newEditFeature.Shape = geometry;
                                                    newEditFeature.Store();
                                                    OnLayerChanged(layer);
                                                    sketch.Geometry = null;
                                                    sketch.RefreshSketch();
                                                    ArcUtils.ActiveView.Refresh();
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                // do nothing
                                            }
                                        }
                                        else
                                            ArcUtils.Editor.StopEditing(false);
                                    }
                                    else
                                    {
                                        // New measurement from map
                                        sketch.Geometry = ConvertToPolyline(coords, layer.HasZ) as ESRI.ArcGIS.Geometry.IGeometry;
                                        sketch.RefreshSketch();
                                        ArcUtils.ActiveView.Refresh();

                                        LastEditedObject = newEditFeature?.OID ?? -1;
                                    }
                                }
                                else
                                {
                                    sketch.Geometry = ConvertToPolyline(coords, layer.HasZ) as ESRI.ArcGIS.Geometry.IGeometry;
                                    sketch.RefreshSketch();
                                    ArcUtils.ActiveView.Refresh();

                                    LastEditedObject = newEditFeature?.OID ?? -1;
                                }
                            }

                            break;

                        case GeometryType.Polygon:
                            var polygon = feature.Geometry as StreetSmart.Common.Interfaces.GeoJson.IPolygon;

                            if (polygon != null)
                            {
                                var sketch = editor as IEditSketch3;
                                IEnumFeature editSelection = ArcUtils.Editor?.EditSelection;
                                editSelection?.Reset();
                                var newEditFeature = editSelection.Next();

                                if (isNew)
                                {
                                    if (sketch != null && sketch.Geometry != null && !sketch.Geometry.IsEmpty)
                                    {
                                        // New measurement from Street Smart
                                        if ((sketch.Geometry as Polygon).PointCount > 1)
                                        {
                                            var geometry = sketch.Geometry;

                                            try
                                            {
                                                (geometry as IZAware).ZAware = layer.HasZ;

                                                if (!_fromSelection)
                                                {
                                                    _fromSelection = false;
                                                    newEditFeature = layer._featureClass.CreateFeature();
                                                    newEditFeature.Shape = geometry;
                                                    newEditFeature.Store();
                                                    OnLayerChanged(layer);
                                                    sketch.Geometry = null;
                                                    sketch.RefreshSketch();
                                                    ArcUtils.ActiveView.Refresh();
                                                }
                                                else if (LastEditedObject != -1 && LastEditedObject == (newEditFeature?.OID ?? -1))
                                                {
                                                    _fromSelection = false;
                                                    newEditFeature.Shape = geometry;
                                                    newEditFeature.Store();
                                                    OnLayerChanged(layer);
                                                    sketch.Geometry = null;
                                                    sketch.RefreshSketch();
                                                    ArcUtils.ActiveView.Refresh();
                                                }
                                                else if (newEditFeature == null && LastEditedObject == -1)
                                                {
                                                    _fromSelection = false;
                                                    newEditFeature = layer._featureClass.CreateFeature();
                                                    newEditFeature.Shape = geometry;
                                                    newEditFeature.Store();
                                                    OnLayerChanged(layer);
                                                    sketch.Geometry = null;
                                                    sketch.RefreshSketch();
                                                    ArcUtils.ActiveView.Refresh();
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                // do nothing
                                            }
                                        }
                                        else
                                            ArcUtils.Editor.StopEditing(false);
                                    }
                                    else
                                    {
                                        // New measurement from map
                                        var newPolygon = new ESRI.ArcGIS.Geometry.Polygon();

                                        foreach (var coordinate in polygon[0])
                                        {
                                            var pointInPolygon = ConvertToPoint(coordinate, layer.HasZ);
                                            newPolygon.AddPoint(pointInPolygon);
                                        }

                                        if (sketch != null && newPolygon.PointCount > 0)
                                        {
                                            (newPolygon as IZAware).ZAware = true;
                                            sketch.Geometry = (ESRI.ArcGIS.Geometry.IGeometry)newPolygon;
                                            sketch.RefreshSketch();
                                        }

                                        ArcUtils.ActiveView.Refresh();

                                        LastEditedObject = newEditFeature?.OID ?? -1;
                                    }
                                }
                                else
                                {
                                    var newPolygon = new ESRI.ArcGIS.Geometry.Polygon();

                                    foreach (var coordinate in polygon[0])
                                    {
                                        var pointInPolygon = ConvertToPoint(coordinate, layer.HasZ);
                                        newPolygon.AddPoint(pointInPolygon);
                                    }

                                    if (sketch != null && newPolygon.PointCount >= 0)
                                    {
                                        (newPolygon as IZAware).ZAware = true;
                                        sketch.Geometry = (ESRI.ArcGIS.Geometry.IGeometry)newPolygon;
                                        sketch.RefreshSketch();
                                    }

                                    ArcUtils.ActiveView.Refresh();

                                    LastEditedObject = newEditFeature?.OID ?? -1;
                                }
                            }

                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }

                StreetSmartApiWrapper.Instance.BusyForMeasurement = false;
            }
        }

        #endregion functions (static)

        #region functions (public)

        // =========================================================================
        // Functions (Public)
        // =========================================================================

        public void SetVisibility(bool isVisible)
        {
            OnLayerChanged(this);

            IEditor3 editor = ArcUtils.Editor;

            if (editor != null)
            {
                UpdateCurrentLayer(editor as IEditLayers, isVisible);
                UpdateEditFeature(editor as IEditSketch3, isVisible);
            }
        }

        public void UpdateCurrentLayer(IEditLayers editor, bool isVisible)
        {
            if (editor != null && editor.CurrentLayer != null)
            {
                VectorLayer vectorLayer = GetLayer(editor.CurrentLayer);

                if (vectorLayer == this)
                {
                    if (isVisible)
                    {
                        OnSketchModified();
                    }
                    else if (CheckEditTask() && SketchFinishedEvent != null)
                    {
                        SketchFinishedEvent();
                        AvContentChanged();
                    }
                }
            }
        }

        public void UpdateEditFeature(IEditSketch3 editor, bool isVisible)
        {
            foreach (var editFeature in EditFeatures)
            {
                VectorLayer vectorLayer = GetLayer(editFeature);

                if (vectorLayer == this)
                {
                    if (isVisible)
                    {
                        var sketch = editor as IEditSketch3;

                        if (sketch?.Geometry != null)
                        {
                            var typeOfLayer = GetTypeOfLayer(sketch.Geometry.GeometryType);

                            StartMeasurementEvent(typeOfLayer);
                        }
                    }
                    else
                    {
                        FeatureDeleteEvent(editFeature);
                    }
                }
            }
        }

        public Color GetColor(out Color outline)
        {
            outline = Color.Black;

            if (_layer != null)
                return ArcUtils.GetColorFromLayer(_layer, out outline);
            else
                return Color.Black;
        }

        public ISymbolizer CreateSymbolizer(Color color, Color outline)
        {
            switch (TypeOfLayer)
            {
                case TypeOfLayer.Point:
                    return SLDFactory.CreateStylePoint(SymbolizerType.Circle, 15, color, 75, outline, 0);

                case TypeOfLayer.Line:
                    return SLDFactory.CreateStyleLine(color);

                case TypeOfLayer.Polygon:
                    return SLDFactory.CreateStylePolygon(color, 75);

                default:
                    return default(ISymbolizer);
            }
        }

        public IStyledLayerDescriptor CreateSld(IFeatureCollection featureCollection, Color color, Color outline)
        {
            // according to specs, we only need color.
            if (featureCollection.Features.Count >= 1)
            {
                Sld = SLDFactory.CreateEmptyStyle();

                var symbolizer = CreateSymbolizer(color, outline);
                var rule = SLDFactory.CreateRule(symbolizer);

                SLDFactory.AddRuleToStyle(Sld, rule);
            }

            return Sld;
        }

        public IFeatureCollection GenerateJson(IList<IRecording> recordingLocations)
        {
            var spatRel = Config.SpatialReference;
            string srsName = (spatRel == null) ? ArcUtils.EpsgCode : spatRel.SRSName;
            string layerName = _layer.Name;
            int srs = int.Parse(srsName.Replace("EPSG:", string.Empty));
            var featureCollection = GeoJsonFactory.CreateFeatureCollection(srs);

            var featureLayer = _layer as IFeatureLayer;
            if (featureLayer != null)
            {
                IFeatureClass fc = featureLayer.FeatureClass;

                ESRI.ArcGIS.Geometry.IGeometry geometryBag = new GeometryBag();
                var geometryCollection = geometryBag as IGeometryCollection;
                ISpatialReference gsSpatialReference = (spatRel == null) ? ArcUtils.SpatialReference : spatRel.SpatialRef;
                ISpatialReference cycloramaSRS = new SpatialReferenceEnvironmentClass().CreateSpatialReference(Config.ApiSSRAsInt);

                double distance = CalculateDistance(gsSpatialReference);

                foreach (var recordingLocation in recordingLocations)
                {
                    var envelope = CreateEnvelope(recordingLocation, distance, cycloramaSRS, SpatialReference);
                    geometryCollection.AddGeometry(envelope);
                }

                ITopologicalOperator unionedPolygon = (ITopologicalOperator)new Polygon();
                unionedPolygon.ConstructUnion(geometryBag as IEnumGeometry);
                var polygon = unionedPolygon as ESRI.ArcGIS.Geometry.IPolygon;

                ISpatialFilter spatialFilter = new SpatialFilter
                {
                    Geometry = polygon,
                    GeometryField = _featureClass.ShapeFieldName,
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects
                };

                UpdateFeatures(featureCollection, spatialFilter, srs, gsSpatialReference);
            }

            ContentsChanged = featureCollection != _contents;
            _contents = featureCollection;
            return featureCollection;
        }

        public double CalculateDistance(ISpatialReference sourceSpatialReference)
        {
            double distance = Config.OverlayDrawDistanceInMeters * 1.0;

            var projCoord = sourceSpatialReference as IProjectedCoordinateSystem;
            if (projCoord == null)
            {
                var geoCoord = sourceSpatialReference as IGeographicCoordinateSystem;

                if (geoCoord != null)
                    distance *= geoCoord.CoordinateUnit.ConversionFactor;
            }
            else
            {
                distance /= projCoord.CoordinateUnit.ConversionFactor;
            }

            return distance;
        }

        public IEnvelope2 CreateEnvelope(IRecording recordingLocation, double distance, ISpatialReference sourceSpatialReference, ISpatialReference targetSpatialReference)
        {
            double x = recordingLocation.XYZ.X.Value;
            double y = recordingLocation.XYZ.Y.Value;

            var result = new Envelope()
            {
                XMin = x - distance,
                XMax = x + distance,
                YMin = y - distance,
                YMax = y + distance,
            } as IEnvelope2;

            result.SpatialReference = sourceSpatialReference;
            result.Project(targetSpatialReference);

      var testA = sourceSpatialReference.FactoryCode;
      var testB = targetSpatialReference.FactoryCode;
      var testC = "";

            return result;
        }

        public void UpdateFeatures(IFeatureCollection features, ISpatialFilter spatialFilter, int srs, ISpatialReference spatialReference)
        {
            var featureCursor = _featureClass.Search(spatialFilter, false);
            var featureCount = _featureClass.FeatureCount(spatialFilter);
            if(featureCount > 1000) { featureCount = 0; }
            var shapeId = featureCursor.FindField(_featureClass.ShapeFieldName);

            for (int i = 0; i < featureCount; i++)
            {
                var feature = featureCursor.NextFeature();

                if (!EditFeatures.Contains(feature))
                {
                    var fields = feature.Fields;
                    var fieldValues = new Dictionary<string, string> { { "FEATURECLASSNAME", _featureClass.AliasName } };

                    for (int j = 0; j < fields.FieldCount; j++)
                    {
                        var field = fields.Field[j];
                        string name = field.Name;
                        int id = featureCursor.FindField(name);

                        string value = (id != shapeId)
                          ? feature.get_Value(id).ToString()
                          : _featureClass.ShapeType.ToString().Replace("esriGeometry", string.Empty);
                        fieldValues.Add(name, value);
                    }

                    var shapeVar = feature.get_Value(shapeId);
                    var geometry = shapeVar as ESRI.ArcGIS.Geometry.IGeometry;
                    if (geometry != null)
                    {
                        var cyclSpatialRef = new SpatialReferenceEnvironmentClass().CreateSpatialReference(srs);
                        geometry.Project((Config.ApiSRS == null) ? spatialReference : cyclSpatialRef);

                        var pointCollection = geometry as IPointCollection4;

                        if (pointCollection != null)
                        {
                            if (TypeOfLayer == TypeOfLayer.Line)
                            {
                                var pointCollectionJson = new List<ICoordinate>();
                                for (int j = 0; j < pointCollection.PointCount; j++)
                                {
                                    IPoint point = pointCollection.Point[j];
                                    AddPoint(pointCollectionJson, point);
                                }

                                var points = new List<IList<ICoordinate>> { pointCollectionJson };

                                if (points.Count > 0)
                                {
                                    var geomJson = GeoJsonFactory.CreateLineFeature(points.FirstOrDefault());
                                    features.Features.Add(geomJson);
                                }
                            }
                            else if (TypeOfLayer == TypeOfLayer.Polygon)
                            {
                                var points = new List<IList<ICoordinate>> ();
                                var polygon = geometry as IPolygon4;

                                try
                                {
                                    if (polygon != null)
                                    {
                                        IGeometryBag extRingGeomBag = polygon.ExteriorRingBag;
                                        IGeometryCollection extRingGeomColl = extRingGeomBag as IGeometryCollection;

                                        for (int l = 0; l < extRingGeomColl?.GeometryCount; l++)
                                        {
                                            var extPointCollectionJson = new List<ICoordinate>();
                                            ESRI.ArcGIS.Geometry.IGeometry
                                                extRingGeom = extRingGeomColl.get_Geometry(l);
                                            IPointCollection extRingPointColl = extRingGeom as IPointCollection;

                                            for (int m = 0; m < extRingPointColl.PointCount; m++)
                                            {
                                                IPoint point = extRingPointColl.get_Point(m);
                                                AddPoint(extPointCollectionJson, point);
                                            }

                                            points.Add(extPointCollectionJson);

                                            IGeometryBag intRingGeomBag =
                                                polygon.get_InteriorRingBag(extRingGeom as IRing);
                                            IGeometryCollection intRingGeomColl = intRingGeomBag as IGeometryCollection;

                                            for (int n = 0; n < intRingGeomColl.GeometryCount; n++)
                                            {
                                                ESRI.ArcGIS.Geometry.IGeometry
                                                    intRing = intRingGeomColl.get_Geometry(n);
                                                IPointCollection intRingPointColl = intRing as IPointCollection;
                                                var intPointCollectionJson = new List<ICoordinate>();

                                                for (int o = 0; o < intRingPointColl.PointCount; o++)
                                                {
                                                    IPoint point = intRingPointColl.get_Point(o);
                                                    AddPoint(intPointCollectionJson, point);
                                                }

                                                points.Add(intPointCollectionJson);
                                            }
                                        }
                                    }

                                    var geomJson = GeoJsonFactory.CreatePolygonFeature(points);
                                    features.Features.Add(geomJson);
                                }
                                catch (Exception)
                                {
                                    // exception do nothing
                                }
                            }
                        }
                        else
                        {
                            var point = geometry as ESRI.ArcGIS.Geometry.IPoint;

                            if (point != null)
                            {
                                ICoordinate pJson = HasZ
                                    ? CoordinateFactory.Create(point.X, point.Y, point.Z)
                                    : CoordinateFactory.Create(point.X, point.Y);
                                var geomJson = GeoJsonFactory.CreatePointFeature(pJson);
                                features.Features.Add(geomJson);
                            }
                        }
                    }

                    foreach (var fieldValue in fieldValues)
                    {
                        if (features.Features.Count >= 1)
                        {
                            var properties = features.Features[features.Features.Count - 1].Properties;

                            if (!properties.ContainsKey(fieldValue.Key))
                                properties.Add(fieldValue.Key, fieldValue.Value);
                        }
                    }
                }
            }
        }

        private void AddPoint(List<ICoordinate> pointCollectionJson, IPoint point)
        {
            if (point != null)
            {
                ICoordinate pJson = HasZ
                    ? CoordinateFactory.Create(point.X, point.Y, point.Z)
                    : CoordinateFactory.Create(point.X, point.Y);
                pointCollectionJson.Add(pJson);
            }
        }

        #endregion functions (public)

        #region event handlers

        // =========================================================================
        // Event handlers
        // =========================================================================
        private static void OpenDocument()
        {
            try
            {
                DetectVectorLayers(false);

                var avEvents = ArcUtils.ActiveViewEvents;

                if (avEvents != null)
                {
                    avEvents.ItemAdded += AvItemAdded;
                    avEvents.ItemDeleted += AvItemDeleted;
                    avEvents.ContentsChanged += AvContentChanged;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "VectorLayer.OpenDocument");
            }
        }

        private static void CloseDocument()
        {
            try
            {
                var avEvents = ArcUtils.ActiveViewEvents;

                if (avEvents != null)
                {
                    avEvents.ItemAdded -= AvItemAdded;
                    avEvents.ItemDeleted -= AvItemDeleted;
                    avEvents.ContentsChanged -= AvContentChanged;
                }

                while (Layers.Count >= 1)
                {
                    bool deleted = AvDoItemDeleted(Layers[0]._layer);

                    if (!deleted)
                    {
                        Layers.RemoveAt(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "VectorLayer.CloseDocument");
            }
        }

        private static void AvItemAdded(object item)
        {
            try
            {
                if (item != null)
                {
                    var featureLayer = item as IFeatureLayer;
                    var extension = StreetSmartExtension.GetExtension();

                    if ((featureLayer != null) && (extension != null))
                    {
                        CycloMediaGroupLayer cycloGrouplayer = extension.CycloMediaGroupLayer;

                        if (cycloGrouplayer != null)
                        {
                            if (!cycloGrouplayer.IsKnownName(featureLayer.Name))
                            {
                                var dataset = item as IDataset;

                                if (dataset != null)
                                {
                                    var featureWorkspace = dataset.Workspace as IFeatureWorkspace;

                                    if (featureWorkspace != null)
                                    {
                                        var featureClass = featureLayer.FeatureClass;

                                        var vectorLayer = new VectorLayer
                                        {
                                            _featureClass = featureClass,
                                            _layer = featureLayer,
                                            IsVisibleInStreetSmart = StoredLayers.Instance.Get(featureLayer.Name)
                                        };

                                        _layers.Add(vectorLayer);

                                        LayerAddEvent?.Invoke(vectorLayer);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "VectorLayer.AvItemAdded");
            }
        }

        private static void AvItemDeleted(object item)
        {
            AvDoItemDeleted(item);
        }

        private static bool AvDoItemDeleted(object item)
        {
            bool result = false;

            try
            {
                if (item != null)
                {
                    var featureLayer = item as IFeatureLayer;

                    if (featureLayer != null)
                    {
                        int i = 0;

                        while (Layers.Count > i)
                        {
                            var layer = Layers[i]._layer as IFeatureLayer;

                            if (layer?.FeatureClass == featureLayer.FeatureClass)
                            {
                                LayerRemoveEvent?.Invoke(Layers[i]);

                                Layers.RemoveAt(i);
                                result = true;
                            }
                            else
                            {
                                i++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "VectorLayer.AvItemDeleted");
            }

            return result;
        }

        private static void AvContentChanged()
        {
            OnLayerChanged(null);

            _layers = new List<VectorLayer>();
            IMap map = ArcUtils.Map;

            if (map != null)
            {
                var layers = map.get_Layers();
                ILayer layer;

                while ((layer = layers.Next()) != null)
                {
                    AvItemAdded(layer);
                }
            }
        }

        private static void OnStartEditing()
        {
            LogClient.Info("On StartEditing");

            ArcUtils.Editor?.Map?.ClearSelection();
        }

        private static void OnLayerChanged(VectorLayer layer)
        {
            try
            {
                LayerChangedEvent?.Invoke(layer);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "VectorLayer.AvContentChanged");
            }
        }

        private static async void OnChangeFeature(IObject obj)
        {
            try
            {
                var feature = obj as ESRI.ArcGIS.Geodatabase.IFeature;
                LogClient.Info(string.Format("On Change Feature: {0}", ((feature != null) ? feature.Class.AliasName : string.Empty)));

                if ((FeatureUpdateEditEvent != null) && (feature != null))
                {
                    if (!EditFeatures.Contains(feature))
                    {
                        EditFeatures.Add(feature);
                    }

                    VectorLayer vectorLayer = GetLayer(feature);

                    if ((vectorLayer != null) && (vectorLayer.IsVisibleInStreetSmart))
                    {
                        FeatureUpdateEditEvent(feature);
                        AvContentChanged();
                        await OnSelectionChanged(true);
                    }
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("VectorLayer.OnChangeFeature", ex.Message, ex);
                Trace.WriteLine(ex.Message, "VectorLayer.OnChangeFeature");
            }
        }

        private static async void OnSelectionChanged()
        {
            _fromSelection = true;
            await OnSelectionChanged(true);
        }

        private static async Task OnSelectionChanged(bool includeStart)
        {
            try
            {
                IApplication application = ArcMap.Application;
                ICommandItem tool = application.CurrentTool;
                string name = tool.Name;

                IEditor3 editor = ArcUtils.Editor;
                LogClient.Info("On Selection Changed");

                if (editor != null)
                {
                    IEnumFeature editSelection = editor.EditSelection;

                    if (editSelection != null)
                    {
                        var editLayers = editor as IEditLayers;
                        editSelection.Reset();
                        EditFeatures.Clear();
                        IFeature feature;
                        TypeOfLayer typeOfLayer = GetTypeOfLayer(editLayers?.CurrentLayer?.FeatureClass?.ShapeType ?? esriGeometryType.esriGeometryNull);

                        while ((feature = editSelection.Next()) != null)
                        {
                            typeOfLayer = GetTypeOfLayer(feature.Shape.GeometryType);

                            if (includeStart || typeOfLayer == TypeOfLayer.Point)
                            {
                                EditFeatures.Add(feature);
                            }
                        }

                        if (!StreetSmartApiWrapper.Instance.BusyForMeasurement && ((EditFeatures.Count > 0 || typeOfLayer != TypeOfLayer.Point) && includeStart || !includeStart))
                        {
                            await StreetSmartApiWrapper.Instance.CreateMeasurement(typeOfLayer);

                            if (!StreetSmartApiWrapper.Instance.BusyForMeasurement) //the await can remove the selection in between, so recheck to be sure.
                            {
                                StreetSmartApiWrapper.Instance.UpdateActiveMeasurement(EditFeatures.Count > 0 ? EditFeatures[0].Shape : null, typeOfLayer);
                            }
                        }
                        if (FeatureStartEditEvent != null)
                        {
                            var geometries = new List<ESRI.ArcGIS.Geometry.IGeometry>();
                            editSelection.Reset();
                            bool isPointLayer = false;

                            while ((feature = editSelection.Next()) != null)
                            {
                                VectorLayer vectorLayer = GetLayer(feature);

                                if ((vectorLayer != null) && (vectorLayer.IsVisibleInStreetSmart))
                                {
                                    geometries.Add(feature.Shape);
                                    //isPointLayer = isPointLayer || (Measurement.GetTypeOfLayer(feature.Shape) == TypeOfLayer.Point); // TODO: Measurement
                                }
                            }

                            if ((_doSelection && (name != "Query_SelectFeatures")) || (!isPointLayer))
                            {
                                //TODO: I think this is breaking more then it fixes...
                                //FeatureStartEditEvent?.Invoke(geometries);

                                AvContentChanged();

                                _doSelection = false;

                                if (_nextSelectionTimer == null)
                                {
                                    var checkEvent = new AutoResetEvent(true);
                                    const int timeOutTime = 1500;

                                    var checkTimerCallBack = new TimerCallback(CheckTimerCallBack);

                                    _nextSelectionTimer = new Timer(checkTimerCallBack, checkEvent, timeOutTime, -1);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("VectorLayer.OnSelectionChanged", ex.Message, ex);
                Trace.WriteLine(ex.Message, "VectorLayer.OnSelectionChanged");
            }
        }

        private static void CheckTimerCallBack(object state)
        {
            _doSelection = true;
            _nextSelectionTimer = null;
        }

        private static async void OnDeleteFeature(IObject obj)
        {
            try
            {
                var feature = obj as ESRI.ArcGIS.Geodatabase.IFeature;
                LogClient.Info(string.Format("On Delete Feature: {0}", ((feature != null) ? feature.Class.AliasName : string.Empty)));

                if (FeatureDeleteEvent != null && feature != null)
                {
                    if (EditFeatures.Contains(feature))
                    {
                        EditFeatures.Remove(feature);
                    }

                    VectorLayer vectorLayer = GetLayer(feature);

                    if ((vectorLayer != null) && (vectorLayer.IsVisibleInStreetSmart))
                    {
                        await OnSelectionChanged(false);
                        var editor = ArcUtils.Editor as IEditLayers;
                        var sketch = editor as IEditSketch3;
                        sketch.Geometry = null;
                        sketch.RefreshSketch();
                        ArcUtils.ActiveView.Refresh();

                        FeatureDeleteEvent(feature);
                        AvContentChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("VectorLayer.OnDeleteFeature", ex.Message, ex);
                Trace.WriteLine(ex.Message, "VectorLayer.OnDeleteFeature");
            }
        }

        private static void OnStopEditing(bool save)
        {
            try
            {
                LogClient.Info(string.Format("On Stop Editing: {0}", save));
                EditFeatures.Clear();
                _doSelection = true;

                if (StopEditEvent != null)
                {
                    StopEditEvent();
                    AvContentChanged();
                }

                if (_editToolCheckTimer != null)
                {
                    _editToolCheckTimer.Stop();
                    _editToolCheckTimer.Dispose();
                    _editToolCheckTimer = null;
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("VectorLayer.OnStopEditing", ex.Message, ex);
                Trace.WriteLine(ex.Message, "VectorLayer.OnStopEditing");
            }
        }

        private static void OnSketchModified()
        {
            try
            {
                LogClient.Info("On Sketch Modified");
                _doSelection = true;

                var avEvents = ArcUtils.ActiveViewEvents;
                avEvents.AfterDraw -= AvEvents_AfterDraw;
                avEvents.AfterDraw += AvEvents_AfterDraw;

                if (_editToolCheckTimer == null)
                {
                    const int checkTime = 1000;
                    _editToolCheckTimer = new System.Windows.Forms.Timer {Interval = checkTime};
                    _editToolCheckTimer.Tick+= EditToolCheck;
                    _editToolCheckTimer.Start();
                }

                IEditor3 editor = ArcUtils.Editor;

                if (editor != null)
                {
                    var sketch = editor as IEditSketch3;
                    var editLayers = editor as IEditLayers;

                    if (sketch != null && editLayers != null)
                    {
                        ILayer currentLayer = editLayers.CurrentLayer;
                        VectorLayer vectorLayer = GetLayer(currentLayer);

                        if (vectorLayer != null && vectorLayer.IsVisibleInStreetSmart && CheckEditTask())
                        {
                            var geometry = sketch.Geometry;
                            var lastPoint = sketch.LastPoint;

                            if (lastPoint != null && lastPoint.Z == 0 && SketchCreateEvent != null && sketch.ZAware)
                                SketchCreateEvent(sketch);

                            if (!StreetSmartApiWrapper.Instance.BusyForMeasurement)
                                SketchModifiedEvent?.Invoke(geometry);

                            _oldEnvelope = _newEnvelope;
                            var envelope = sketch.Geometry.Envelope;
                            IActiveView activeView = ArcUtils.ActiveView;
                            var display = activeView.ScreenDisplay;
                            IDisplayTransformation dispTrans = display.DisplayTransformation;
                            double size = dispTrans.FromPoints(14);

                            _newEnvelope = new EnvelopeClass
                            {
                                XMin = envelope.XMin - size, XMax = envelope.XMax + size, YMin = envelope.YMin - size,
                                YMax = envelope.YMax + size
                            };

                            if (_oldEnvelope != null)
                            {
                                display.Invalidate(_oldEnvelope, true, (short)esriScreenCache.esriNoScreenCache);
                            }

                            display.Invalidate(_newEnvelope, true, (short)esriScreenCache.esriNoScreenCache);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("VectorLayer.OnSketchModified", ex.Message, ex);
                Trace.WriteLine(ex.Message, "VectorLayer.OnSketchModified");
            }
        }

        private static async void OnSketchFinished()
        {
            try
            {
                IEditor3 editor = ArcUtils.Editor;
                LogClient.Info("On Sketch Finished");
                _doSelection = true;

                if (editor != null)
                {
                    var editLayers = editor as IEditLayers;

                    if (editLayers != null)
                    {
                        ILayer currentLayer = editLayers.CurrentLayer;
                        VectorLayer vectorLayer = GetLayer(currentLayer);

                        if (CheckEditTask() && vectorLayer != null && vectorLayer.IsVisibleInStreetSmart)
                        {
                            if (SketchFinishedEvent != null)
                            {
                                _fromSelection = false;
                                SketchFinishedEvent();
                                AvContentChanged();
                            }

                            await OnSelectionChanged(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("VectorLayer.OnSketchFinished", ex.Message, ex);
                Trace.WriteLine(ex.Message, "VectorLayer.OnSketchFinished");
            }
        }

        #endregion event handlers

        #region thread functions

        // =========================================================================
        // Thread functions
        // =========================================================================
        private static void EditToolCheck(object sender, EventArgs e)
        {
            try
            {
                lock (LockObject)
                {
                    IEditor3 editor = ArcUtils.Editor;
                    IApplication application = ArcMap.Application;
                    IActiveView activeView = ArcUtils.ActiveView;

                    if ((application != null) && (activeView != null) && (editor != null))
                    {
                        ICommandItem tool = application.CurrentTool;

                        if ((tool != null) && ((_beforeTool == null) || (_beforeTool.Name != tool.Name)))
                        {
                            _beforeTool = tool;
                            var editLayers = editor as IEditLayers;

                            if (editLayers != null)
                            {
                                ILayer currentLayer = editLayers.CurrentLayer;
                                VectorLayer vectorLayer = (currentLayer == null) ? null : GetLayer(currentLayer);

                                if ((vectorLayer != null) && (vectorLayer.IsVisibleInStreetSmart))
                                {
                                    ICommandItem editorMenu = application.Document.CommandBars.Find("Editor_EditTool");
                                    string editorCategory = (editorMenu != null) ? editorMenu.Category : string.Empty;
                                    ICommand command = tool.Command;
                                    string category = tool.Category;

                                    if (category == editorCategory)
                                    {
                                        UpdateEditGeometry(editor, command);
                                    }
                                }
                                else
                                {
                                    SketchFinishedEvent?.Invoke();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "VectorLayer.OnEditToolCheck");
            }
        }

        private static void AvViewRefreshed(IActiveView view, esriViewDrawPhase phase, object data, IEnvelope envelope)
        {
            try
            {
                IEditor3 editor = ArcUtils.Editor;
                IApplication application = ArcMap.Application;

                if ((editor != null) && (application != null))
                {
                    var editLayers = editor as IEditLayers;

                    if (editLayers != null)
                    {
                        ILayer currentLayer = editLayers.CurrentLayer;
                        VectorLayer vectorLayer = (currentLayer == null) ? null : GetLayer(currentLayer);
                        ICommandItem tool = application.CurrentTool;

                        if ((tool != null) && ((_beforeTool == null) || (_beforeTool.Name != tool.Name)))
                        {
                            _beforeTool = tool;

                            if ((vectorLayer != null) && (vectorLayer.IsVisibleInStreetSmart))
                            {
                                ICommandItem editorMenu = application.Document.CommandBars.Find("Editor_EditTool");
                                string editorCategory = (editorMenu != null) ? editorMenu.Category : string.Empty;
                                ICommand command = tool.Command;
                                string category = tool.Category;

                                if (category == editorCategory)
                                {
                                    UpdateEditGeometry(editor, command);
                                }
                            }
                            else
                            {
                                SketchFinishedEvent?.Invoke();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "VectorLayer.AvViewRefreshed");
            }
        }

        #endregion thread functions
    }
}