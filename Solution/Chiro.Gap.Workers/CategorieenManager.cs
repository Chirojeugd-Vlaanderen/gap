// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. categorieën bevat
    /// </summary>
    public class CategorieenManager
    {
        private readonly IAutorisatieManager _autorisatieMgr;

        public CategorieenManager(IAutorisatieManager auMgr)
        {
            _autorisatieMgr = auMgr;
        }
    }
}