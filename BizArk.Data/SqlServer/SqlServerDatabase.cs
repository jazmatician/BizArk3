﻿using BizArk.Core.Extensions.StringExt;
using BizArk.Data.SqlServer.SqlClientExt;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BizArk.Data.SqlServer
{

	/// <summary>
	/// Derived from `BaDatabase` to support Microsoft Sql Server.
	/// </summary>
	public class SqlServerDatabase : BaDatabase
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of SqlServerDatabase.
		/// </summary>
		public SqlServerDatabase(string connStr)
		{
			if (connStr.IsEmpty()) throw new ArgumentNullException(nameof(connStr));
			ConnectionString = connStr;
		}

		/// <summary>
		/// Disposes the SqlServerDatabase instance.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			// Once the database has been disposed, we shouldn't need this anymore.
			// Null it out so it will fail if anybody attempts to access it after 
			// it's been disposed.
			ConnectionString = null;

			base.Dispose(disposing);
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Error code for deadlocks in Sql Server.
		/// </summary>
		internal const int cSqlError_Deadlock = 1205;

		// Internal so it can be viewed in the unit tests.
		internal string ConnectionString { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Instantiates a SqlConnection.
		/// </summary>
		/// <returns></returns>
		protected override DbConnection InstantiateConnection()
		{
			return new SqlConnection(ConnectionString);
		}

		/// <summary>
		/// Displays the SqlCommand.DebugText in the debug window.
		/// </summary>
		/// <param name="cmd"></param>
		protected override void PrepareCommand(DbCommand cmd)
		{
			var sqlcmd = cmd as SqlCommand;
			if (sqlcmd != null)
				Debug.WriteLine(sqlcmd.DebugText());

			base.PrepareCommand(cmd);
		}
		
		/// <summary>
		/// Gets the schema for the table.
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		protected override DataTable GetSchema(DbConnection conn, string tableName)
		{
			using (var da = new SqlDataAdapter($"SELECT * FROM {tableName} WHERE 0 = 1", conn as SqlConnection))
			{
				var ds = new DataSet();
				da.FillSchema(ds, SchemaType.Source, tableName);
				return ds.Tables[tableName];
			}
		}

		/// <summary>
		/// Retry on deadlock.
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		protected override bool ShouldRetry(Exception ex)
		{
			var sqlex = ex as SqlException;
			if (sqlex == null) return false;
			return sqlex.ErrorCode == cSqlError_Deadlock;
		}

		#endregion

	}
}
