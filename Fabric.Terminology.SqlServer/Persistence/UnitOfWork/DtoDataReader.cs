namespace Fabric.Terminology.SqlServer.Persistence.UnitOfWork
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;

    internal class DtoDataReader<T> : DbDataReader
        where T : class
    {
        private readonly IReadOnlyCollection<T> dtos;

        private readonly IEnumerator<T> enumerator;

        private readonly List<PropertyInfo> properties = new List<PropertyInfo>();

        private readonly Dictionary<string, int> nameLookup = new Dictionary<string, int>();

        private DtoDataReader(IEnumerable<T> dtos)
        {
            this.dtos = dtos.ToList();
            this.enumerator = this.dtos.GetEnumerator();

            this.properties.AddRange(
                typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly));

            for (var i = 0; i < this.properties.Count; i++)
            {
                var name = this.properties[i].Name.Equals("CodeSystemGuid", StringComparison.OrdinalIgnoreCase)
                               ? "CodeSystemGUID"
                               : this.properties[i].Name;

                this.nameLookup[name] = i;
            }
        }

        public override int FieldCount => this.properties.Count;

        public override int RecordsAffected => this.dtos.Count;

        public override bool HasRows => this.dtos != null && this.dtos.Any();

        public override bool IsClosed => this.enumerator == null;

        public override object this[int ordinal] => this.GetValue(ordinal);

        public override object this[string name] => this.GetValue(this.GetOrdinal(name));

        public static DtoDataReader<T> Create(IEnumerable<T> dtos) => new DtoDataReader<T>(dtos);

        public override bool Read() => this.enumerator.MoveNext();

        public override IEnumerator GetEnumerator() => this.enumerator;

        public override int GetOrdinal(string name)
        {
          return this.nameLookup.ContainsKey(name) ? this.nameLookup[name] : -1;
        } 

        public override object GetValue(int ordinal) => this.properties[ordinal].GetValue(this.enumerator.Current, null);

        public override int GetValues(object[] values)
        {
            var getValues = Math.Max(this.FieldCount, values.Length);

            for (var i = 0; i < getValues; i++)
            {
                values[i] = this.GetValue(i);
            }

            return getValues;
        }

        public override string GetName(int ordinal) => this.properties[ordinal].Name;

        public override Type GetFieldType(int ordinal) => this.properties[ordinal].PropertyType;

        public override bool IsDBNull(int ordinal) => this.GetValue(ordinal) == null;

        public override bool GetBoolean(int ordinal) => (bool)this.GetValue(ordinal);

        public override byte GetByte(int ordinal) => (byte)this.GetValue(ordinal);

        public override char GetChar(int ordinal) => (char)this.GetValue(ordinal);

        public override DateTime GetDateTime(int ordinal) => (DateTime)this.GetValue(ordinal);

        public override decimal GetDecimal(int ordinal) => (decimal)this.GetValue(ordinal);

        public override double GetDouble(int ordinal) => (double)this.GetValue(ordinal);

        public override float GetFloat(int ordinal) => (float)this.GetValue(ordinal);

        public override Guid GetGuid(int ordinal) => (Guid)this.GetValue(ordinal);

        public override short GetInt16(int ordinal) => (short)this.GetValue(ordinal);

        public override int GetInt32(int ordinal) => (int)this.GetValue(ordinal);

        public override long GetInt64(int ordinal) => (long)this.GetValue(ordinal);

        public override string GetString(int ordinal) => (string)this.GetValue(ordinal);

    #pragma warning disable SA1124 // Do not use regions

        #region Not Implemented

    #pragma warning disable SA1201 // Elements must appear in the correct order
        public override int Depth { get; }
    #pragma warning restore SA1201 // Elements must appear in the correct order

        public override bool NextResult()
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)

        {
            throw new NotImplementedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        #endregion

    #pragma warning restore SA1124 // Do not use regions
    }
}
