﻿/*
 * Integration in ArcMap for StreetSmart
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

namespace StreetSmartArcMap.Logic
{
    public interface IStreetSmartOptions
    {
        string ApiSRS { get; set; }
        string AddressLocale { get; set; }
        string AddressDatabase { get; set; }
        string ApiUsername { get; set; }
        string ApiPassword { get; set; }

        string StreetSmartLocationToUse { get; }
        string RecordingsServiceUrlToUse { get; }
        string SpatialReferencesUrlToUse { get; }
        string ConfigurationUrlToUse { get; }
        string LocaleToUse { get; }

        bool UseDefaultStreetSmartLocation { get; set; }
        bool UseDefaultBaseUrl { get; set; }
    }
}