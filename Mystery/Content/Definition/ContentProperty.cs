
using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Mystery.Content
{

    public class ContentProperty : MysteryPropertyAttribute
    {
        public string name { get; set; }
        public string label { get; set; }


        public override void setup()
        {
            if (string.IsNullOrEmpty(name)) {
                name = used_in.Name;
            }
            if (string.IsNullOrEmpty(label))
                label = name;
        }

    }

}