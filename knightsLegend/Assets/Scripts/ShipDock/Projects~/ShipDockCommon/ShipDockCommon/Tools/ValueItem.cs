namespace ShipDock.Tools
{
    public class ValueItem
    {
        public const int STRING = 1;
        public const int INT = 2;
        public const int DOUBLE = 3;
        public const int BOOL = 4;
        public const int FLOAT = 5;

        public readonly static string TRUE_VALUE = true.ToString();
        public readonly static string FALSE_VALUE = false.ToString();
        
        public static ValueItem New(string value = "")
        {
            return new ValueItem(value);
        }
        
        public static ValueItem New(string keyField, string value = "")
        {
            ValueItem result = new ValueItem(value)
            {
                KeyField = keyField
            };
            return result;
        }

        public static ValueItem New(string keyField, float value)
        {
            ValueItem result = new ValueItem(value)
            {
                KeyField = keyField
            };
            return result;
        }

        public static ValueItem New(string keyField, bool value = default)
        {
            ValueItem result = new ValueItem(value)
            {
                KeyField = keyField
            };
            return result;
        }

        public static ValueItem New(string keyField, int value)
        {
            ValueItem result = new ValueItem(value)
            {
                KeyField = keyField
            };
            return result;
        }

        public static ValueItem New(string keyField, double value)
        {
            ValueItem result = new ValueItem(value)
            {
                KeyField = keyField
            };
            return result;
        }

        private bool mBool;
        private int mInt;
        private float mFloat;
        private double mDouble;
        private string mValue;

        public virtual int Int
        {
            get
            {
                return mInt;
            }
            set
            {
                Type = INT;
                Change(value.ToString());
            }
        }

        public virtual float Float
        {
            get
            {
                return mFloat;
            }
            set
            {
                Type = FLOAT;
                Change(value.ToString());
            }
        }

        public virtual double Double
        {
            get
            {
                return mDouble;
            }
            set
            {
                Type = DOUBLE;
                Change(value.ToString());
            }
        }

        public virtual bool Bool
        {
            get
            {
                return mBool;
            }
            set
            {
                Type = BOOL;
                Change(value ? TRUE_VALUE : FALSE_VALUE);
            }
        }

        public bool IsBool
        {
            get
            {
                return Type == BOOL;
            }
        }

        public bool IsFloat
        {
            get
            {
                return Type == FLOAT;
            }
        }

        public bool IsInt
        {
            get
            {
                return Type == INT;
            }
        }

        public virtual int Type { get; set; }
        public string Value { get; private set; }
        public string KeyField { get; set; }
        public float DampTime { get; set; }

        public ValueItem() : this(string.Empty) { }

        public ValueItem(string value)
        {
            Type = STRING;
            Change(value);
        }

        public ValueItem(bool value)
        {
            Bool = value;
        }

        public ValueItem(int value)
        {
            Int = value;
        }

        public ValueItem(float value)
        {
            Float = value;
        }

        public ValueItem(double value)
        {
            Double = value;
        }

        public ValueItem SetDampTime(float time)
        {
            DampTime = time;
            return this;
        }

        public void Change(string value)
        {
            if (!value.Equals(Value))
            {
                Value = value;
                
                if (!bool.TryParse(Value, out mBool))
                {
                    mBool = default;
                }
                if (!int.TryParse(Value, out mInt))
                {
                    mInt = default;
                }
                if (!float.TryParse(Value, out mFloat))
                {
                    mFloat = default;
                }
                if (!double.TryParse(Value, out mDouble))
                {
                    mDouble = default;
                }
            }
        }
    }
}
