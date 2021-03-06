//------------------------------------------------------------------------------
// <copyright file="NameValueCollection.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

/*
 * Ordered String/String[] collection of name/value pairs with support for null key
 * Wraps NameObject collection
 *
 * Copyright (c) 2000 Microsoft Corporation
 */

namespace System.Collections.Specialized {

    using Microsoft.Win32;
    using System.Collections;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Globalization;
    
    /// <devdoc>
    /// <para>Represents a sorted collection of associated <see cref='System.String' qualify='true'/> keys and <see cref='System.String' qualify='true'/> values that 
    ///    can be accessed either with the hash code of the key or with the index.</para>
    /// </devdoc>
    [Serializable()]
    public class NameValueCollection : NameObjectCollectionBase {
    
        private String[] _all;                            
        private String[] _allKeys;
    
        //
        // Constructors
        //
    
        /// <devdoc>
        /// <para>Creates an empty <see cref='System.Collections.Specialized.NameValueCollection'/> with the default initial capacity
        ///    and using the default case-insensitive hash code provider and the default
        ///    case-insensitive comparer.</para>
        /// </devdoc>
        public NameValueCollection() : base() {
        }
    
        /// <devdoc>
        /// <para>Copies the entries from the specified <see cref='System.Collections.Specialized.NameValueCollection'/> to a new <see cref='System.Collections.Specialized.NameValueCollection'/> with the same initial capacity as 
        ///    the number of entries copied and using the default case-insensitive hash code
        ///    provider and the default case-insensitive comparer.</para>
        /// </devdoc>
        public NameValueCollection(NameValueCollection col) 
            : base( col != null? col.Comparer : null) {
            Add(col);
        }
    
        /// <devdoc>
        /// <para>Creates an empty <see cref='System.Collections.Specialized.NameValueCollection'/> with the default initial capacity 
        ///    and using the specified case-insensitive hash code provider and the specified
        ///    case-insensitive comparer.</para>
        /// </devdoc>
#pragma warning disable 618
        [Obsolete("Please use NameValueCollection(IEqualityComparer) instead.")]
        public NameValueCollection(IHashCodeProvider hashProvider, IComparer comparer) 
            : base(hashProvider, comparer) {
        }
#pragma warning restore 618
    
        /// <devdoc>
        /// <para>Creates an empty <see cref='System.Collections.Specialized.NameValueCollection'/> with 
        ///    the specified initial capacity and using the default case-insensitive hash code
        ///    provider and the default case-insensitive comparer.</para>
        /// </devdoc>
        public NameValueCollection(int capacity) : base(capacity) {
        }
    
        public NameValueCollection(IEqualityComparer equalityComparer) : base( equalityComparer) {  
        }

        public NameValueCollection(Int32 capacity, IEqualityComparer equalityComparer)
            : base(capacity, equalityComparer) {
        }

        /// <devdoc>
        /// <para>Copies the entries from the specified <see cref='System.Collections.Specialized.NameValueCollection'/> to a new <see cref='System.Collections.Specialized.NameValueCollection'/> with the specified initial capacity or the 
        ///    same initial capacity as the number of entries copied, whichever is greater, and
        ///    using the default case-insensitive hash code provider and the default
        ///    case-insensitive comparer.</para>
        /// </devdoc>
        public NameValueCollection(int capacity, NameValueCollection col) 
            : base(capacity, (col != null ? col.Comparer : null)) {
            if( col == null) {
                throw new ArgumentNullException("col");
            }

            this.Comparer = col.Comparer;
            Add(col);
        }
    
        /// <devdoc>
        /// <para>Creates an empty <see cref='System.Collections.Specialized.NameValueCollection'/> with the specified initial capacity and 
        ///    using the specified case-insensitive hash code provider and the specified
        ///    case-insensitive comparer.</para>
        /// </devdoc>
#pragma warning disable 618
        [Obsolete("Please use NameValueCollection(Int32, IEqualityComparer) instead.")]
        public NameValueCollection(int capacity, IHashCodeProvider hashProvider, IComparer comparer) 
            : base(capacity, hashProvider, comparer) {
        }
#pragma warning restore 618

        // Allow internal extenders to avoid creating the hashtable/arraylist.
        internal NameValueCollection(DBNull dummy) : base(dummy)
        {
        }


        //
        // Serialization support
        //

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected NameValueCollection(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    
        //
        //  Helper methods
        //
    
        /// <devdoc>
        /// <para> Resets the cached arrays of the collection to <see langword='null'/>.</para>
        /// </devdoc>
        protected void InvalidateCachedArrays() {
            _all = null;
            _allKeys = null;
        }
    
        private static String GetAsOneString(ArrayList list) {
            int n = (list != null) ? list.Count : 0;
    
            if (n == 1) {
                return (String)list[0];
            }
            else if (n > 1) {
                StringBuilder s = new StringBuilder((String)list[0]);
    
                for (int i = 1; i < n; i++) {
                    s.Append(',');
                    s.Append((String)list[i]);
                }
    
                return s.ToString();
            }
            else {
                return null;
            }
        }
    
        private static String[] GetAsStringArray(ArrayList list)
        {
            int n = (list != null) ? list.Count : 0;
            if (n == 0)
                return null;
    
            String [] array = new String[n];
            list.CopyTo(0, array, 0, n);
            return array;
        }
    
        //
        // Misc public APIs
        //
    
        /// <devdoc>
        /// <para>Copies the entries in the specified <see cref='System.Collections.Specialized.NameValueCollection'/> to the current <see cref='System.Collections.Specialized.NameValueCollection'/>.</para>
        /// </devdoc>
        public void Add(NameValueCollection c) {
            if( c == null) {
                throw new ArgumentNullException("c");
            }

            InvalidateCachedArrays();
    
            int n = c.Count;
    
            for (int i = 0; i < n; i++) {
                String key = c.GetKey(i);
                String[] values = c.GetValues(i);
    
                if (values != null) {
                    for (int j = 0; j < values.Length; j++)
                        Add(key, values[j]);
                }
                else {
                    Add(key, null);
                }
            }
        }
    
        /// <devdoc>
        ///    <para>Invalidates the cached arrays and removes all entries 
        ///       from the <see cref='System.Collections.Specialized.NameValueCollection'/>.</para>
        /// </devdoc>
        public virtual void Clear() {
            if (IsReadOnly)
                throw new NotSupportedException(SR.GetString(SR.CollectionReadOnly));
    
            InvalidateCachedArrays();
            BaseClear();
        }
        
        public void CopyTo(Array dest, int index) {
            if (dest==null) {
                throw new ArgumentNullException("dest");
            }

            if (dest.Rank != 1) {
                throw new ArgumentException(SR.GetString(SR.Arg_MultiRank));
            }

            if (index < 0) {
                throw new ArgumentOutOfRangeException("index",SR.GetString(SR.IndexOutOfRange, index.ToString(CultureInfo.CurrentCulture)) );
            }

            if (dest.Length - index < Count) {
                throw new ArgumentException(SR.GetString(SR.Arg_InsufficientSpace));
            }

            int n = Count;
            if (_all == null) {
                String[] all = new String[n];
                for (int i = 0; i < n; i++) {
                    all[i] = Get(i);
                    dest.SetValue( all[i], i + index);
                }
                _all = all; // wait until end of loop to set _all reference in case Get throws
            }
            else {
                for (int i = 0; i < n; i++) {
                    dest.SetValue( _all[i], i + index);
                }                
            }
        }
    
        /// <devdoc>
        /// <para>Gets a value indicating whether the <see cref='System.Collections.Specialized.NameValueCollection'/> contains entries whose keys are not <see langword='null'/>.</para>
        /// </devdoc>
        public bool HasKeys() {
            return InternalHasKeys();
        }

        /// <devdoc>
        /// <para>Allows derived classes to alter HasKeys().</para>
        /// </devdoc>
        internal virtual bool InternalHasKeys()
        {
            return BaseHasKeys();
        }
    
        //
        // Access by name
        //
    
        /// <devdoc>
        ///    <para>Adds an entry with the specified name and value into the 
        ///    <see cref='System.Collections.Specialized.NameValueCollection'/>.</para>
        /// </devdoc>
        public virtual void Add(String name, String value) {
            if (IsReadOnly)
                throw new NotSupportedException(SR.GetString(SR.CollectionReadOnly));
    
            InvalidateCachedArrays();
    
            ArrayList values = (ArrayList)BaseGet(name);
    
            if (values == null) {
                // new key - add new key with single value
                values = new ArrayList(1);
                if (value != null)
                    values.Add(value);
                BaseAdd(name, values);
            }
            else {
                // old key -- append value to the list of values
                if (value != null)
                    values.Add(value);
            }
        }
    
        /// <devdoc>
        /// <para> Gets the values associated with the specified key from the <see cref='System.Collections.Specialized.NameValueCollection'/> combined into one comma-separated list.</para>
        /// </devdoc>
        public virtual String Get(String name) {
            ArrayList values = (ArrayList)BaseGet(name);
            return GetAsOneString(values);
        }
    
        /// <devdoc>
        /// <para>Gets the values associated with the specified key from the <see cref='System.Collections.Specialized.NameValueCollection'/>.</para>
        /// </devdoc>
        public virtual String[] GetValues(String name) {
            ArrayList values = (ArrayList)BaseGet(name);
            return GetAsStringArray(values);
        }
    
        /// <devdoc>
        /// <para>Adds a value to an entry in the <see cref='System.Collections.Specialized.NameValueCollection'/>.</para>
        /// </devdoc>
        public virtual void Set(String name, String value) {
            if (IsReadOnly)
                throw new NotSupportedException(SR.GetString(SR.CollectionReadOnly));
    
            InvalidateCachedArrays();
    
            ArrayList values = new ArrayList(1);
            values.Add(value);
            BaseSet(name, values);
        }
    
        /// <devdoc>
        /// <para>Removes the entries with the specified key from the <see cref='System.Collections.Specialized.NameObjectCollectionBase'/> instance.</para>
        /// </devdoc>
        public virtual void Remove(String name) {
            InvalidateCachedArrays();
            BaseRemove(name);
        }
    
        /// <devdoc>
        ///    <para> Represents the entry with the specified key in the 
        ///    <see cref='System.Collections.Specialized.NameValueCollection'/>.</para>
        /// </devdoc>
        public String this[String name] {
            get {
                return Get(name);
            }
    
            set {
                Set(name, value);
            }
        }
    
        //
        // Indexed access
        //
    
        /// <devdoc>
        ///    <para> 
        ///       Gets the values at the specified index of the <see cref='System.Collections.Specialized.NameValueCollection'/> combined into one
        ///       comma-separated list.</para>
        /// </devdoc>
        public virtual String Get(int index)   {
            ArrayList values = (ArrayList)BaseGet(index);
            return GetAsOneString(values);
        }
    
        /// <devdoc>
        ///    <para> Gets the values at the specified index of the <see cref='System.Collections.Specialized.NameValueCollection'/>.</para>
        /// </devdoc>
        public virtual String[] GetValues(int index) {
            ArrayList values = (ArrayList)BaseGet(index);
            return GetAsStringArray(values);
        }
    
        /// <devdoc>
        /// <para>Gets the key at the specified index of the <see cref='System.Collections.Specialized.NameValueCollection'/>.</para>
        /// </devdoc>
        public virtual String GetKey(int index) {
            return BaseGetKey(index);
        }
    
        /// <devdoc>
        /// <para>Represents the entry at the specified index of the <see cref='System.Collections.Specialized.NameValueCollection'/>.</para>
        /// </devdoc>
        public String this[int index] {
            get {
                return Get(index);
            }
        }
    
        //
        // Access to keys and values as arrays
        //
        
        /// <devdoc>
        /// <para>Gets all the keys in the <see cref='System.Collections.Specialized.NameValueCollection'/>. </para>
        /// </devdoc>
        public virtual String[] AllKeys {
            get {
                if (_allKeys == null)
                    _allKeys = BaseGetAllKeys();
                return _allKeys;
            }
        }
    }

}
