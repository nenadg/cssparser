using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// System.IO is needed for file operations
using System.IO;

namespace CssParser.Models
{
    public class CssProperty
    {
        public string name { get; set; }
        public string type { get; set; }
        public IList<CssValue> values { get; set; }
    }

    public class CssValue
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Parser
    {
        /// <summary>
        /// Parses CSS file
        /// </summary>
        /// <param name="reader">StreamReader with our file</param>
        public void ParseCssFile(System.IO.StreamReader reader, IEnumerable<CssProperty> tochange)
        {

            // This could've been solved in so many ways. Here I'm going to 
            // deconstruct CSS file into our models just for MVC pattern's sake
            List<CssProperty> cssProp = new List<CssProperty>();
            List<string> lines = new List<string>();

            // Main dissadvantage of this parser is that it reads CSS file
            // line by line (which means that if you have expanded CSS this parser is useless)
            // USAGE : you have to compact your CSS file, each line should represent one element 
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // By simple string manipulation, we are going to dissect our template CSS file
                // to find classes, ids and elements
                if (!String.IsNullOrWhiteSpace(line))
                {
                    string[] chunk = line.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);

                    string classOrId = chunk[0];
                    string[] attributes = chunk[1].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    CssProperty cssClass = new CssProperty();

                    // find classes
                    if (chunk[0].StartsWith(".", StringComparison.InvariantCultureIgnoreCase))
                    {
                        cssClass = new CssProperty() { name = chunk[0].Split('.')[1].Trim(), type = "class", values = new List<CssValue>() };
                    }
                    // find ids
                    else if (chunk[0].StartsWith("#", StringComparison.InvariantCultureIgnoreCase))
                    {
                        cssClass = new CssProperty() { name = chunk[0].Split('#')[1].Trim(), type = "id", values = new List<CssValue>() };
                    }
                    // elements appear to have no 'prefix'
                    else
                        cssClass = new CssProperty() { name = chunk[0].Trim(), type = "element", values = new List<CssValue>() };

                    foreach (var attribute in attributes)
                    {
                        if (!String.IsNullOrWhiteSpace(attribute))
                        {
                            string[] _name = attribute.Split(':');
                            cssClass.values.Add(new CssValue { name = _name[0].Trim(), value = _name[1].Trim() });
                        }
                    }

                    if (cssClass.name != null && cssClass.type != null)
                        cssProp.Add(cssClass);
                }
            }

            // ... and now, let's construct our new CSS file.
            foreach (var prop in cssProp)
            {
                string name = "";

                // Recreate valid CSS names upon values find in our model classes
                switch (prop.type)
                {
                    case ("class"):
                        name = "." + prop.name;
                        break;
                    case ("id"):
                        name = "#" + prop.name;
                        break;
                    case ("element"):
                        name = prop.name;
                        break;
                }

                // Recreate values of CSS attributes
                foreach (var attribute in prop.values)
                {
                    string values = "";

                    // Since you probably won't set all CSS attributes every time
                    // we have to differentiate the ones you do from ones you don's,
                    // and since I'm to bored with this project to figure out a better way
                    // try...catch will do.
                    // We're going to try setting value and if it fails that would mean that
                    // you haven't tried to set it at the first place, so 'catch' will 
                    // set it as it was in the template file.
                    try
                    {
                        values = tochange.FirstOrDefault(n => n.name == prop.name).values.FirstOrDefault(v => v.name == attribute.name).value + "; ";
                    }
                    catch
                    {
                        values = attribute.value + "; ";
                    }
                    finally
                    {
                        if (lines.Any(p => p.StartsWith(name)))
                        {
                            lines[lines.FindIndex(p => p.StartsWith(name))] += attribute.name + ": " + values;
                        }
                        else
                            lines.Add((name + " { " + attribute.name + ": " + values));
                    }
                }
            }

            reader.Close();

            // Write our newly created file
            using (StreamWriter file = new StreamWriter(Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "static/css/demo-new.css")))
            {
                foreach (string lineInFile in lines)
                {
                    file.WriteLine(lineInFile + " }");
                }
                file.Close();
            }
        }
    }
}