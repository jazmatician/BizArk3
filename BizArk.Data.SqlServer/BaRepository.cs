﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizArk.Data.SqlServer
{

    /// <summary>
    /// The base class for classes that access the database. A repository should have fairly simple database commands and no business logic.
    /// </summary>
    public class BaRepository : IDisposable, ISupportBaDatabase
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates a new instance of BaRepository creating a new instance of BaDatabase.
        /// </summary>
		/// <param name="name">The name or key of the connection string in the config file.</param>
        public BaRepository(string name)
            : this(BaDatabase.Create(name))
        {
            DisposeDatabase = true;
        }

        /// <summary>
        /// Creates a new instance of BaRepository.
        /// </summary>
        /// <param name="db">The database to use for the repository. The database will not be disposed with the repository.</param>
        public BaRepository(ISupportBaDatabase db)
        {
            if (db == null) throw new ArgumentNullException("db");
            if (db.Database == null) throw new ArgumentException("Unable to access the database.", "db");

            DisposeDatabase = false;
            Database = db.Database;
        }

        /// <summary>
        /// Cleans up any resources that the repository is using.
        /// </summary>
        public void Dispose()
        {
            if (Database != null)
            {
                if (DisposeDatabase)
                    Database.Dispose();

                Database = null;
            }
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets or sets a value that determines if the database should be disposed when the repository is disposed.
        /// </summary>
        public bool DisposeDatabase { get; set; }

        /// <summary>
        /// Gets the database for this repository instance.
        /// </summary>
        public BaDatabase Database { get; private set; }

        #endregion

    }
}
