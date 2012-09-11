﻿using MediaBrowser.Controller.Entities.TV;
using System.IO;
using System.Xml;

namespace MediaBrowser.Controller.Providers.TV
{
    public class EpisodeXmlParser : BaseItemXmlParser<Episode>
    {
        protected override void FetchDataFromXmlNode(XmlReader reader, Episode item)
        {
            switch (reader.Name)
            {
                case "filename":
                    {
                        string filename = reader.ReadElementContentAsString();

                        if (!string.IsNullOrWhiteSpace(filename))
                        {
                            string seasonFolder = Path.GetDirectoryName(item.Path);
                            item.PrimaryImagePath = Path.Combine(seasonFolder, "metadata", filename);
                        }
                        break;
                    }
                case "SeasonNumber":
                    {
                        string number = reader.ReadElementContentAsString();

                        if (!string.IsNullOrWhiteSpace(number))
                        {
                            item.ParentIndexNumber = int.Parse(number);
                        }
                        break;
                    }

                case "EpisodeNumber":
                    {
                        string number = reader.ReadElementContentAsString();

                        if (!string.IsNullOrWhiteSpace(number))
                        {
                            item.IndexNumber = int.Parse(number);
                        }
                        break;
                    }

                case "EpisodeName":
                    item.Name = reader.ReadElementContentAsString();
                    break;

                default:
                    base.FetchDataFromXmlNode(reader, item);
                    break;
            }
        }
    }
}
