using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDLibrary.UnitsOfMeasure
{
    internal sealed class UnitPartCollection : ICollection<UnitPart>
    {
        public UnitPartCollection(IEnumerable<UnitPart> parts)
        {
            foreach (var part in parts)
                Add(part);
        }

        public UnitPartCollection()
        {

        }

        private Dictionary<Unit, UnitPart> parts = new Dictionary<Unit, UnitPart>();

        public void Add(Unit unit, int exponent)
        {
            DerivedUnit du = unit as DerivedUnit;
            if (du != null)
                foreach (UnitPart part in du.Parts)
                    Add(new UnitPart(part.Unit, part.Exponent * exponent));
            else
                Add(new UnitPart(unit, exponent));
        }

        public void Add(UnitPart item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item.Unit.Equals(Unit.Dimensionless))
                return;

            UnitPart old;
            if (!parts.TryGetValue(item.Unit, out old))
                parts.Add(item.Unit, item);
            else
            {
                parts.Remove(old.Unit);
                int exponent = old.Exponent + item.Exponent;
                if (exponent != 0)
                    parts.Add(old.Unit, new UnitPart(old.Unit, exponent));
            }
        }

        public void Clear()
        {
            parts.Clear();
        }

        public bool Contains(UnitPart item)
        {
            return parts.ContainsKey(item.Unit);
        }

        public void CopyTo(UnitPart[] array, int arrayIndex)
        {
            parts.Values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return parts.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Unit unit, int exponent)
        {
            Add(unit, -exponent);

            return true;
        }

        public bool Remove(UnitPart item)
        {
            Add(new UnitPart(item.Unit, -item.Exponent));

            return true;
        }

        public IEnumerator<UnitPart> GetEnumerator()
        {
            return parts.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
