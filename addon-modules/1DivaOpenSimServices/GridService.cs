﻿/**
 * Copyright (c) Marck. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 * 
 *     * Redistributions of source code must retain the above copyright notice, 
 *       this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright notice, 
 *       this list of conditions and the following disclaimer in the documentation 
 *       and/or other materials provided with the distribution.
 *     * Neither the name of the Organizations nor the names of Individual
 *       Contributors may be used to endorse or promote products derived from 
 *       this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL 
 * THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, 
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
 * GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED 
 * AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 */

using System;
using System.Reflection;

using log4net;
using Nini.Config;
using OpenMetaverse;

using OpenSim.Services.Interfaces;
using GridRegion = OpenSim.Services.Interfaces.GridRegion;
using RegionFlags = OpenSim.Framework.RegionFlags;

namespace Diva.OpenSimServices
{
    public class GridService : OpenSim.Services.GridService.GridService, IGridService
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string m_CastWarning = "[DivaData]: Invalid cast for Grid store. Diva.Data required for method {0}.";
        
        public GridService(IConfigSource config) : base(config) 
        {
        }

        public GridRegion TryLinkRegionToCoords(UUID scopeID, string address, uint xloc, uint yloc, UUID ownerID, out string reason)
        {
            return m_HypergridLinker.TryLinkRegionToCoords(scopeID, address, (int)xloc, (int)yloc, ownerID, out reason);
        }

        public bool TryUnlinkRegion(string mapName)
        {
            return m_HypergridLinker.TryUnlinkRegion(mapName);
        }

        public long GetLocalRegionsCount(UUID scopeID)
        {
            try
            {
                // fkb DreamGrid SmartStart mod to show smart boot regions on home page
                const RegionFlags flags = RegionFlags.RegionOnline | RegionFlags.Persistent;
                const RegionFlags excludeFlags = RegionFlags.Hyperlink;
                return ((Diva.Data.IRegionData)m_Database).GetCount(scopeID, (int)flags, (int)excludeFlags);
            }
            catch (InvalidCastException)
            {
                m_log.WarnFormat(m_CastWarning, MethodBase.GetCurrentMethod().Name);
            }
            return 0;
        }

        public void SetFallback(UUID regionID, Boolean fallback)
        {
            OpenSim.Data.RegionData rdata = m_Database.Get(regionID, UUID.Zero);
            if (rdata != null)
            {
                int flags = Convert.ToInt32(rdata.Data["flags"]);
                if (fallback)
                    flags |= (int)OpenSim.Framework.RegionFlags.FallbackRegion;
                else
                {
                    int mask = 1 << (int)Math.Log((double)OpenSim.Framework.RegionFlags.FallbackRegion, 2); 
                    flags &= ~mask;
                }
                rdata.Data["flags"] = flags.ToString();
                m_Database.Store(rdata);
            }
        }

    }
}
