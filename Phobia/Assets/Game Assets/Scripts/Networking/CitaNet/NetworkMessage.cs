using UnityEngine;
using System.Collections.Generic;

namespace CitaNet
{
    public class NetworkMessage
    {
        public const char RECORD_SEPARATOR = (char)30;
        public const char UNIT_SEPARATOR = (char)31;

        public float sendTime;

        private Dictionary<string, string> messageParts = new Dictionary<string, string>();

        public NetworkMessage()
        {

        }

        public NetworkMessage(string unparsed)
        {
            string[] records = unparsed.Split(RECORD_SEPARATOR);

            foreach(string r in records)
            {
                string[] units = r.Split(UNIT_SEPARATOR);

                if (units.Length != 2)
                    continue;

                messageParts.Add(units[0], units[1]);
            }
        }

        public override string ToString()
        {
            string message = "";

            foreach(KeyValuePair<string, string> kvp in messageParts)
            {
                message += kvp.Key + UNIT_SEPARATOR + kvp.Value + RECORD_SEPARATOR;
            }

            return message;
        }

        public void setFloat(string key, float value)
        {
            Debug.Assert(!key.Contains(RECORD_SEPARATOR.ToString()) && !key.Contains(UNIT_SEPARATOR.ToString()), "Key cannot contain record or unit seperator characters.");
            messageParts[key] = value.ToString();
        }

        public bool getFloat(string key, out float result)
        {
            string value;

            if (messageParts.TryGetValue(key, out value))
            {
                return float.TryParse(value, out result);
            }
            else
            {
                result = 0;
                return false;
            }
        }

        public void setInt(string key, int value)
        {
            Debug.Assert(!key.Contains(RECORD_SEPARATOR.ToString()) && !key.Contains(UNIT_SEPARATOR.ToString()), "Key cannot contain record or unit seperator characters.");
            messageParts[key] = value.ToString();
        }

        public bool getInt(string key, out int result)
        {
            string value;

            if (messageParts.TryGetValue(key, out value))
            {
                return int.TryParse(value, out result);
            }
            else
            {
                result = 0;
                return false;
            }
        }

        public void setBool(string key, bool value)
        {
            Debug.Assert(!key.Contains(RECORD_SEPARATOR.ToString()) && !key.Contains(UNIT_SEPARATOR.ToString()), "Key cannot contain record or unit seperator characters.");
            messageParts[key] = value.ToString();
        }

        public bool getBool(string key, out bool result)
        {
            string value;

            if (messageParts.TryGetValue(key, out value))
            {
                return bool.TryParse(value, out result);
            }
            else
            {
                result = false;
                return false;
            }
        }

        public void setString(string key, string value)
        {
            Debug.Assert(!key.Contains(RECORD_SEPARATOR.ToString()) && !key.Contains(UNIT_SEPARATOR.ToString()), "Key cannot contain record or unit seperator characters.");
            Debug.Assert(!value.Contains(RECORD_SEPARATOR.ToString()) && !value.Contains(UNIT_SEPARATOR.ToString()), "Value cannot contain record or unit seperator characters.");
            messageParts[key] = value;
        }

        public bool getString(string key, out string result)
        {
            return messageParts.TryGetValue(key, out result);
        }
    }
}
