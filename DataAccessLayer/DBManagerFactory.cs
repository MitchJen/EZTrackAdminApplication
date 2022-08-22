using lcpi.data.oledb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Text;
using Npgsql;
namespace DataAccessLayer
{
    public sealed class DBManagerFactory
    {
        private DBManagerFactory() { }

        public static IDbConnection GetConnection(DataProvider providerType)
        {
            IDbConnection iDbConnection = null;
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    iDbConnection = new SqlConnection();
                    break;
                case DataProvider.OleDb:
                    iDbConnection = new OleDbConnection();
                    break;
                case DataProvider.Odbc:
                    iDbConnection = new OdbcConnection();
                    break;
                case DataProvider.Pgsql:
                    iDbConnection = new Npgsql.NpgsqlConnection();
                    break;

                default:
                    return null;
            }
            return iDbConnection;
        }

        public static IDbCommand GetCommand(DataProvider providerType)
        {
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    return new SqlCommand();
                case DataProvider.OleDb:
                    return new OleDbCommand();
                case DataProvider.Odbc:
                    return new OdbcCommand();
                case DataProvider.Pgsql:
                    return new NpgsqlCommand();

                default:
                    return null;
            }
        }

        public static IDbDataAdapter GetDataAdapter(DataProvider
        providerType)
        {
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    return new SqlDataAdapter();
                case DataProvider.OleDb:
                    return new OleDbDataAdapter();
                case DataProvider.Odbc:
                    return new OdbcDataAdapter();
                case DataProvider.Pgsql:
                    return new NpgsqlDataAdapter();

                default:
                    return   null;
            }
        }

        public static IDbTransaction GetTransaction(DataProvider
         providerType)
        {
            IDbConnection iDbConnection = GetConnection(providerType);

            IDbTransaction iDbTransaction = iDbConnection.BeginTransaction();
            return iDbTransaction;
        }

        public static IDataParameter GetParameter(DataProvider
         providerType)
        {
            IDataParameter iDataParameter = null;
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    iDataParameter = new SqlParameter();
                    break;
                case DataProvider.OleDb:
                    iDataParameter = new OleDbParameter();
                    break;
                case DataProvider.Odbc:
                    iDataParameter = new OdbcParameter();
                    break;
                case DataProvider.Pgsql:
                    iDataParameter = new NpgsqlParameter();
                    break;

            }
            return iDataParameter;
        }

        public static DbCommandBuilder GetCommandBuilder(DataProvider
         providerType, IDataAdapter cmdDataAdaptor)
        {
            DbCommandBuilder iDbCommandBuilder = null;
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    iDbCommandBuilder = new SqlCommandBuilder((SqlDataAdapter)cmdDataAdaptor);

                    break;
                case DataProvider.OleDb:
                    iDbCommandBuilder = new OleDbCommandBuilder((OleDbDataAdapter)cmdDataAdaptor);
                    break;
                case DataProvider.Odbc:
                    iDbCommandBuilder = new OdbcCommandBuilder((OdbcDataAdapter)cmdDataAdaptor);
                    break;
                case DataProvider.Pgsql:
                    iDbCommandBuilder = new NpgsqlCommandBuilder((NpgsqlDataAdapter)cmdDataAdaptor);
                    break;

            }
            return iDbCommandBuilder;
        }



        public static IDbDataParameter[] GetParameters(DataProvider
         providerType,
          int paramsCount)
        {
            IDbDataParameter[] idbParams = new IDbDataParameter[paramsCount];

            switch (providerType)
            {
                case DataProvider.SqlServer:
                    for (int i = 0; i < paramsCount; ++i)
                    {
                        idbParams[i] = new SqlParameter();
                    }
                    break;
                case DataProvider.OleDb:
                    for (int i = 0; i < paramsCount; ++i)
                    {
                        idbParams[i] = new OleDbParameter();
                    }
                    break;
                case DataProvider.Odbc:
                    for (int i = 0; i < paramsCount; ++i)
                    {
                        idbParams[i] = new OdbcParameter();
                    }
                    break;
                case DataProvider.Pgsql:
                    for (int i = 0; i < paramsCount; ++i)
                    {
                        idbParams[i] = new NpgsqlParameter();
                    }
                    break;
                default:
                    idbParams = null;
                    break;
            }
            return idbParams;
        }
    }
}
