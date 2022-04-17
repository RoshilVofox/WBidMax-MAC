using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [XmlRoot("TagDetails")]
    [Serializable]
    public class TagDetails : List<Tag>
    {

        public TagDetails()
        {

        }

        public TagDetails(TagDetails tagDetails)
        {
            //var tag=new TagDetails()
            if (tagDetails != null && tagDetails.Count!=0)
                {
                    for(int cnt=0;cnt<= tagDetails.Count;cnt++)
                    {
                          this.Add(new Tag(tagDetails[cnt]));
                    }
                }

           

        }



    }
    [DataContract]
    [Serializable]
    public class Tag
    {

        /// <summary>
        /// PURPOSe : Order Id
        /// </summary>
        [DataMember]
        [XmlAttribute("Line")]
        public int Line { get; set; }

        /// <summary>
        /// PURPOSE : Line Id
        /// </summary>
        [DataMember]
        [XmlAttribute("Tag")]
        public string Content { get; set; }

        public Tag()
        {

        }


        public Tag(Tag tag)
        {
            Line = tag.Line;
            Content = tag.Content;

        }
    }


}
