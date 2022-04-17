using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    
    public class PostionConstraint
    {
        public  PostionConstraint()
        {
        }
        public PostionConstraint(PostionConstraint postionConstraint)
        {
            A = postionConstraint.A;
            B = postionConstraint.B;
            C = postionConstraint.C;
            D = postionConstraint.D;

        }
        [XmlAttribute("A")]
        public bool A { get; set; }
        [XmlAttribute("B")]
        public bool B { get; set; }
        [XmlAttribute("C")]
        public bool C { get; set; }
        [XmlAttribute("D")]
        public bool D { get; set; }

    }
}
