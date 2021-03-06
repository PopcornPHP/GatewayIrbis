﻿/* MstRecord.cs
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ManagedClient;

#endregion

namespace ManagedClient
{
    [Serializable]
    [DebuggerDisplay("Leader={Leader}")]
    public sealed class MstRecord
    {
        #region Constants

        #endregion

        #region Properties

        public MstRecordLeader Leader { get; set; }

        public List<MstDictionaryEntry> Dictionary { get; set; }

        public bool Deleted
        {
            get { return ((Leader.Status & (int)(RecordStatus.LogicallyDeleted | RecordStatus.PhysicallyDeleted)) != 0); }
        }

        #endregion

        #region Private members

        private string _DumpDictionary ( )
        {
            StringBuilder result = new StringBuilder();

            foreach ( MstDictionaryEntry entry in Dictionary )
            {
                result.AppendLine ( entry.ToString () );
            }

            return result.ToString ();
        }

        #endregion

        #region Public methods

        public RecordField DecodeField(MstDictionaryEntry entry)
        {
            string catenated = string.Format
                (
                    "{0}#{1}",
                    entry.Tag,
                    entry.Text
                );

            RecordField result = RecordField.Parse(catenated);

            return result;
        }

        public IrbisRecord DecodeRecord()
        {
            IrbisRecord result = new IrbisRecord
                {
                    Mfn = Leader.Mfn,
                    Status = (RecordStatus) Leader.Status,
                    PreviousOffset = Leader.Previous,
                    Version = Leader.Version
                };

            foreach (MstDictionaryEntry entry in Dictionary)
            {
                RecordField field = DecodeField(entry);
                result.Fields.Add(field);
            }

            return result;
        }

        #endregion

        #region Object members

        public override string ToString ( )
        {
            return string.Format 
                ( 
                    "Leader: {0}\r\nDictionary: {1}", 
                    Leader,
                    _DumpDictionary ()
                );
        }

        #endregion
    }
}
