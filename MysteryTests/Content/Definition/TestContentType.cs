using Mystery.Content;
using Mystery.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysteryTests.Content.Definition
{

    public enum a_enum {
        zero,
        one,
        two,
        tree,}

    [ContentType(label = "Test Content", list_label = "Test Contents")]
    [Serializable()]
    public class TestContentType : BaseContent
    {

        [ContentProperty(), PropertyView()]
        public string a_string { get; set; } 

        [ContentProperty(), ReferenceText(), PropertyView(), SearchText()]
        public string the_reference_text { get; set; } 

        [ContentProperty(), PropertyView()]
        public long a_integer { get; set; } 


        [ContentProperty(), PropertyView()]
        public bool  a_boolean { get; set; } 


        [ContentProperty(), PropertyView()]
        public double  a_double { get; set; }

        [ContentProperty(), PropertyView()]
        public a_enum a_enum { get; set; }

        [ContentProperty(), PropertyView(),SingleReferencePropertyValuesProviderAtt(), PropertyEdit()]
        public ContentReference<TestContentType> single_reference { get; set; }

        [ContentProperty(), PropertyView()]
        public ContentReference<TestContentType> not_ediatable_reference { get; set; }

        [ContentProperty(), PropertyView()]
        public MultiContentReference<TestContentType> multi_reference { get; set; } 

        static Random rnd = new Random();

        static public TestContentType getARandomTestContentTypeWithoutreference()
        {
            TestContentType result = new TestContentType();
            result.a_string = Guid.NewGuid().ToString();
            result.the_reference_text = Guid.NewGuid().ToString();
            result.a_integer = rnd.Next();
            result.a_boolean = rnd.NextDouble() > 0.5;
            result.a_double = rnd.NextDouble();
            result.a_enum = (a_enum)rnd.Next(4);
            return result;
        }

        static public TestContentType getARandomTestContentType(bool enforce_a_reference) {
            
            TestContentType result = getARandomTestContentTypeWithoutreference();
            if (enforce_a_reference || rnd.NextDouble() > 0.5) {
                result.single_reference = getARandomTestContentTypeWithoutreference();
            };
            if (enforce_a_reference || rnd.NextDouble() > 0.5)
            {
                result.not_ediatable_reference = getARandomTestContentTypeWithoutreference();
            };

            int number_of_references = rnd.Next(10);
            if (enforce_a_reference) number_of_references += 1;

            result.multi_reference = new MultiContentReference<TestContentType>( 
                (from int i in Enumerable.Range(0,number_of_references)
                 select new ContentReference<TestContentType>(getARandomTestContentTypeWithoutreference()) ));

            return result;
        }

        public bool exposeInit { get {
                return Initializing;
            } }

    }

    [ContentType(label = "Test Content", list_label = "Test Contents")]
    public class TestContentType2 : TestContentType { }

    public class TestNotContentType 
    {
        public string a_string { get; set; } 

        public string the_reference_text { get; set; } 

        public int a_integer { get; set; }

        public bool a_boolean { get; set; } 

        public double  a_double { get; set; } 

        public TestContentType single_reference { get; set; } 

        public List<TestContentType> multi_reference { get; set; } 

        static Random rnd = new Random();


        static public TestNotContentType getARandomTestContentType()
        {
            TestNotContentType result = new TestNotContentType();
            result.a_string = Guid.NewGuid().ToString();
            result.the_reference_text = Guid.NewGuid().ToString();
            result.a_integer = rnd.Next();
            result.a_boolean = rnd.NextDouble() > 0.5;
            result.a_double = rnd.NextDouble();
            
            if (rnd.NextDouble() > 0.5)
            {
                result.single_reference = TestContentType.getARandomTestContentTypeWithoutreference();
            };

            result.multi_reference = new List<TestContentType>(
                (from int i in Enumerable.Range(0, rnd.Next(10))
                 select TestContentType.getARandomTestContentTypeWithoutreference()));

            return result;
        }

    }

}
