using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SmartWalk.Core;
using SmartWalk.Core.Model;
using SmartWalk.Core.Services;
using System.Xml.Linq;
using MonoTouch.CoreText;

namespace SmartWalk.iOS.Services
{
    public class SmartWalkDataService : ISmartWalkDataService
    {
        public SmartWalkDataService()
        {
        }

        public void GetOrgs(Action<IEnumerable<Organization>, Exception> resultHandler)
        {
            try
            {
                var xml = XDocument.Load(@"TempXML/Local/index.xml");
                var result = xml.Descendants("organization").Select(org => 
                    new Organization 
                        {
                            Id = org.Attribute("id").Value,
                            Name = org.Attribute("name").Value,
                            Logo = "TempXML/Local/" + org.Attribute("id").Value + 
                                "/" + org.Attribute("logo").Value,
                        }).ToArray();

                resultHandler(result, null);
            }
            catch (Exception ex)
            {
                resultHandler(null, ex);
            }
        }

        public void GetOrgEvents(string orgId, Action<IEnumerable<OrgEvent>, Exception> resultHandler)
        {
            /*XmlTextReader xml;

            if (orgId == "nbff")
            {
                xml = new XmlTextReader("TempXML/nbff/nbff-2013-06-07.xml");
            }
            else if (orgId == "mapp")
            {
                xml = new XmlTextReader("TempXML/mapp/mapp-2013-06-01.xml");
            }*/
        }
    }
}