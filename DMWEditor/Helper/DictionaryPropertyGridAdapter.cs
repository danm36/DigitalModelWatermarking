using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMWEditor
{
    public class StringKey
    {
        static Dictionary<string, StringKey> skCache = new Dictionary<string, StringKey>();

        public string KeyText { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public bool Editable { get; set; }
        public bool Visible { get; set; }
        public bool Save { get; set; }

        public StringKey(string text, string category, string description, bool editable = true, bool visisble = true, bool save = true)
        {
            KeyText = text;
            Category = category;
            Description = description;
            Editable = editable;
            Visible = visisble;
            Save = save;

            skCache[text] = this;
        }

        public StringKey(string text, string category, bool editable = true, bool visisble = true, bool save = true)
            : this(text, category, "", editable, visisble, save)
        {
        }

        public StringKey(string text, bool editable = true, bool visisble = true, bool save = true)
            : this(text, "Metadata", "", editable, visisble, save)
        {
        }


        public override string ToString()
        {
            return KeyText;
        }

        public static implicit operator string(StringKey s)
        {
            return s.KeyText;
        }

        public static implicit operator StringKey(string s)
        {
            if (skCache.ContainsKey(s))
                return skCache[s];

            return new StringKey(s);
        }
    }

    class DictionaryPropertyGridAdapter : ICustomTypeDescriptor
    {
        IDictionary _dictionary;

        public DictionaryPropertyGridAdapter(IDictionary d)
        {
            _dictionary = d;
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return _dictionary;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            ArrayList properties = new ArrayList();
            int index = 0;
            foreach (DictionaryEntry e in _dictionary)
            {
                if(((StringKey)e.Key).Visible)
                    properties.Add(new DictionaryPropertyDescriptor(_dictionary, e.Key, ++index));
            }

            PropertyDescriptor[] props = (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));

            return new PropertyDescriptorCollection(props);
        }
    }

    class DictionaryPropertyDescriptor : PropertyDescriptor
    {
        IDictionary _dictionary;
        StringKey _key;
        int _index;

        internal DictionaryPropertyDescriptor(IDictionary d, object key, int index)
            : base(key.ToString(), null)
        {
            _dictionary = d;
            _index = index;

            if (key is StringKey)
                _key = key as StringKey;
            else if (key is string)
                _key = new StringKey(key.ToString());
            else
                throw new ArgumentException("Dictonary keys must be of type string or StringKey");
        }

        public override Type PropertyType
        {
            get { return _dictionary[_key].GetType(); }
        }

        public override void SetValue(object component, object value)
        {
            _dictionary[_key] = value;
        }

        public override object GetValue(object component)
        {
            return _dictionary[_key];
        }

        public override bool IsReadOnly
        {
            get { return !_key.Editable; }
        }

        public override Type ComponentType
        {
            get { return null; }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override string DisplayName
        {
            get
            {
                return _key.KeyText;
            }
        }

        public override string Category
        {
            get
            {
                return _key.Category;
            }
        }

        public override string Description
        {
            get
            {
                return _key.Description;
            }
        }

        public override string Name
        {
            get
            {
                return "#" + _index;
            }
        }
    }
}
