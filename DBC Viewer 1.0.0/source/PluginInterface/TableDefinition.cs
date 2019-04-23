using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace PluginInterface
{
    [Serializable]
    public class DBFilesClient
    {
        [XmlElement("Table")]
        public List<Table> Tables { get; set; }

        [XmlIgnore]
        public string File { get; set; }

        public static DBFilesClient Load(string path)
        {
            XmlSerializer deser = new XmlSerializer(typeof(DBFilesClient));
            using (var fs = new FileStream(path, FileMode.Open))
            {
                DBFilesClient cat = (DBFilesClient)deser.Deserialize(fs);
                cat.File = path;
                return cat;
            }
        }

        public static void Save(DBFilesClient db)
        {
            string dir = Path.GetDirectoryName(db.File);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            XmlSerializer ser = new XmlSerializer(typeof(DBFilesClient));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            using (var fs = new FileStream(db.File, FileMode.Create))
                ser.Serialize(fs, db, namespaces);
        }
    }

    [Serializable]
    public class Table
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public int Build { get; set; }
        [XmlElement("Field")]
        public List<Field> Fields { get; set; }

        public Table Clone()
        {
            Table cloned = new Table();
            cloned.Name = Name;
            cloned.Build = Build;
            cloned.Fields = new List<Field>();
            foreach (Field f in Fields)
                cloned.Fields.Add(f.Clone());
            return cloned;
        }
    }

    [Serializable]
    public class Field
    {
        [XmlIgnore]
        public int Index { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Type { get; set; }
        [XmlAttribute, DefaultValue("")]
        public string Format { get; set; } = string.Empty;
        [XmlAttribute, DefaultValue(1)]
        public int ArraySize { get; set; } = 1;
        [XmlAttribute, DefaultValue(false)]
        public bool IsIndex { get; set; } = false;
        [XmlAttribute, DefaultValue(true)]
        public bool Visible { get; set; } = true;
        [XmlAttribute, DefaultValue(100)]
        public int Width { get; set; } = 100;

        public Field Clone()
        {
            return (Field)MemberwiseClone();
        }
    }
}
