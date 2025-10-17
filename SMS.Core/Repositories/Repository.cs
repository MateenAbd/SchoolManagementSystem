using Dapper;
using Microsoft.EntityFrameworkCore;
using SMS.Core.Entities;
using SMS.Core.Exceptions;
using SMS.Core.Interfaces;
using SMS.Core.Logger.Services;
using SMS.Core.Entities;
using SMS.Core.Exceptions;
using SMS.Core.Interfaces;
using SMS.Core.Logger.Services;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace SMS.Core.Repositories
{
    public class Repository : IRepository
    {
        private readonly string _connectionString;
        private readonly int _commandTimeout;
        private readonly LogService _logger;

        /// <summary>
        /// Inject the IConfiguration when creating an instance of this class.
        /// </summary>
        /// <param name="context"></param>
        public Repository(DbContext context)
        {
            _connectionString = context.Database.GetDbConnection().ConnectionString;
            _commandTimeout = (context.Database.GetCommandTimeout() == null) ? 30 : Convert.ToInt32(context.Database.GetCommandTimeout());
            _logger = new LogService();
        }

        private IDbConnection OpenConnection()
        {
            IDbConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// <para>Execute any Stored Procedure where a return data set it not expected.</para>
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure to be executed.</param>
        /// <param name="parameters">Optional set of parameters that matches the query.</param>
        public void ExecuteSp(string storedProcedureName, List<ParametersCollection> parameters = null)
        {
            try
            {
                DynamicParameters procedureParameters = new DynamicParameters();

                if (parameters != null)
                {
                    foreach (ParametersCollection parameter in parameters)
                    {
                        procedureParameters.Add(parameter.ParameterName, parameter.ParameterValue, dbType: parameter.ParameterType, direction: parameter.ParameterDirection);
                    }
                }

                using var connection = OpenConnection();
                connection.Execute(storedProcedureName, param: procedureParameters, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error Running ExecuteSP");
                throw new StoredProcedureExecutionException(ex.Message);
            }
        }

        /// <summary>
        /// <para>Execute any Stored Procedure where a return data set it not expected.</para>
        /// </summary>
        /// <param name="token">Cancellation Token</param>
        /// <param name="storedProcedureName">Name of the stored procedure to be executed.</param>
        /// <param name="parameters">Optional set of parameters that matches the query.</param>
        public async Task ExecuteSpAsync(CancellationToken token, string storedProcedureName, List<ParametersCollection> parameters = null)
        {
            try
            {
                DynamicParameters procedureParameters = new DynamicParameters();

                if (parameters != null)
                {
                    foreach (ParametersCollection parameter in parameters)
                    {
                        procedureParameters.Add(parameter.ParameterName, parameter.ParameterValue, dbType: parameter.ParameterType, direction: parameter.ParameterDirection);
                    }
                }

                using var connection = OpenConnection();
                await connection.ExecuteAsync(new CommandDefinition(commandText: storedProcedureName, cancellationToken: token, parameters: procedureParameters, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error Running ExecuteSPAsync");
                throw new StoredProcedureExecutionException(ex.Message);
            }
        }

        /// <summary>
        /// <para>Execute a Store Procedure when a List of T is expected in return.</para>
        /// </summary>
        /// <typeparam name="T">The type that matches the database table.</typeparam>
        /// <param name="storedProcedureName">Name of the stored procedure to be executed.</param>
        /// <param name="parameters">Optional set of parameters that matches the query.</param>
        /// <returns>An IEnumerable of type T.</returns>
        public IEnumerable<T> ExecuteSpList<T>(string storedProcedureName, List<ParametersCollection> parameters = null)
        {
            try
            {
                DynamicParameters procedureParameters = new DynamicParameters();

                if (parameters != null)
                {
                    foreach (ParametersCollection parameter in parameters)
                    {
                        procedureParameters.Add(parameter.ParameterName, parameter.ParameterValue, dbType: parameter.ParameterType, direction: parameter.ParameterDirection);
                    }
                }

                using var connection = OpenConnection();
                IEnumerable<T> output = connection.Query<T>(storedProcedureName, param: procedureParameters, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout);
                return output;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error Running ExecuteSPList");
                throw new StoredProcedureExecutionException(ex.Message);
            }
        }

        /// <summary>
        /// <para>Execute a Store Procedure when a List of T is expected in return.</para>
        /// </summary>
        /// <typeparam name="T">The type that matches the database table.</typeparam>
        /// <param name="token">Cancellation Token</param>
        /// <param name="storedProcedureName">Name of the stored procedure to be executed.</param>
        /// <param name="parameters">Optional set of parameters that matches the query.</param>
        /// <returns>An IEnumerable of type T.</returns>
        public async Task<IEnumerable<T>> ExecuteSpListAsync<T>(CancellationToken token, string storedProcedureName, List<ParametersCollection> parameters = null)
        {
            try
            {
                DynamicParameters procedureParameters = new DynamicParameters();

                if (parameters != null)
                {
                    foreach (ParametersCollection parameter in parameters)
                    {
                        procedureParameters.Add(parameter.ParameterName, parameter.ParameterValue, dbType: parameter.ParameterType, direction: parameter.ParameterDirection);
                    }
                }

                using var connection = OpenConnection();
                var output = await connection.QueryAsync<T>(new CommandDefinition(commandText: storedProcedureName, cancellationToken: token, parameters: procedureParameters, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout));
                return output;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error Running ExecuteSPListAsync");
                throw new StoredProcedureExecutionException(ex.Message);
            }
        }

        /// <summary>
        /// <para>Execute any Stored Procedure where a return value is expected as a return.</para>
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure to be executed.</param>
        /// <param name="parameters">Optional set of parameters that matches the query.</param>
        /// <returns>long value</returns>
        public long ExecuteSpReturnValue(string storedProcedureName, List<ParametersCollection> parameters = null)
        {
            try
            {
                DynamicParameters procedureParameters = new DynamicParameters();

                if (parameters != null)
                {
                    foreach (ParametersCollection parameter in parameters)
                    {
                        procedureParameters.Add(parameter.ParameterName, parameter.ParameterValue, dbType: parameter.ParameterType, direction: parameter.ParameterDirection);
                    }
                }

                procedureParameters.Add("ReturnValue", dbType: DbType.Int64, direction: ParameterDirection.ReturnValue);

                using (var connection = OpenConnection())
                {
                    connection.Execute(storedProcedureName, param: procedureParameters, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout);
                }

                long returnValue = procedureParameters.Get<int>("ReturnValue");

                return returnValue;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error Running ExecuteSPReturnValue");
                throw new StoredProcedureExecutionException(ex.Message);
            }
        }

        /// <summary>
        /// <para>Execute any Stored Procedure where a return value is expected as a return.</para>
        /// </summary>
        /// <param name="token">Cancellation Token</param>
        /// <param name="storedProcedureName">Name of the stored procedure to be executed.</param>
        /// <param name="parameters">Optional set of parameters that matches the query.</param>
        /// <returns>long value</returns>
        public async Task<long> ExecuteSpReturnValueAsync(CancellationToken token, string storedProcedureName, List<ParametersCollection> parameters = null)
        {
            try
            {
                DynamicParameters procedureParameters = new DynamicParameters();

                if (parameters != null)
                {
                    foreach (ParametersCollection parameter in parameters)
                    {
                        procedureParameters.Add(parameter.ParameterName, parameter.ParameterValue, dbType: parameter.ParameterType, direction: parameter.ParameterDirection);
                    }
                }

                procedureParameters.Add("ReturnValue", dbType: DbType.Int64, direction: ParameterDirection.ReturnValue);

                using (var connection = OpenConnection())
                {
                    await connection.ExecuteAsync(new CommandDefinition(commandText: storedProcedureName, cancellationToken: token, parameters: procedureParameters, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout));
                }

                long returnValue = procedureParameters.Get<int>("ReturnValue");

                return returnValue;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error Running ExecuteSPReturnValueAsync");
                throw new StoredProcedureExecutionException(ex.Message);
            }
        }

        /// <summary>
        /// <para>Execute any Stored Procedure where a single item is expected as a return.</para>
        /// </summary>
        /// <typeparam name="T">The type that matches the database table.</typeparam>
        /// <param name="storedProcedureName">Name of the stored procedure to be executed.</param>
        /// <param name="parameters">Optional set of parameters that matches the query.</param>
        /// <returns>A single instance of type T.</returns>
        public T ExecuteSpSingle<T>(string storedProcedureName, List<ParametersCollection> parameters = null)
        {
            try
            {
                DynamicParameters procedureParameters = new DynamicParameters();

                if (parameters != null)
                {
                    foreach (ParametersCollection parameter in parameters)
                    {
                        procedureParameters.Add(parameter.ParameterName, parameter.ParameterValue, dbType: parameter.ParameterType, direction: parameter.ParameterDirection);
                    }
                }

                T returnSingle;

                using (var connection = OpenConnection())
                {
                    returnSingle = connection.QueryFirstOrDefault<T>(storedProcedureName, param: procedureParameters, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout);

                }
                if (returnSingle != null)
                {
                    return returnSingle;
                }
                else
                {
                    throw new StoredProcedureExecutionException("No Records Found");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error Running ExecuteSPSingle");
                throw new StoredProcedureExecutionException(ex.Message);
            }
        }

        /// <summary>
        /// <para>Execute any Stored Procedure where a single item is expected as a return.</para>
        /// </summary>
        /// <typeparam name="T">The type that matches the database table.</typeparam>
        /// <param name="token">Cancellation Token</param>
        /// <param name="storedProcedureName">Name of the stored procedure to be executed.</param>
        /// <param name="parameters">Optional set of parameters that matches the query.</param>
        /// <returns>A single instance of type T.</returns>
        public async Task<T> ExecuteSpSingleAsync<T>(CancellationToken token, string storedProcedureName, List<ParametersCollection> parameters = null)
        {
            try
            {
                DynamicParameters procedureParameters = new DynamicParameters();

                if (parameters != null)
                {
                    foreach (ParametersCollection parameter in parameters)
                    {
                        procedureParameters.Add(parameter.ParameterName, parameter.ParameterValue, dbType: parameter.ParameterType, direction: parameter.ParameterDirection);
                    }
                }

                T returnSingle;

                using (var connection = OpenConnection())
                {
                    returnSingle = await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(commandText: storedProcedureName, cancellationToken: token, parameters: procedureParameters, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout));
                }
                if (returnSingle != null)
                {
                    return returnSingle;
                }
                else
                {
                    throw new StoredProcedureExecutionException("No Records Found");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error Running ExecuteSPSingleAsync");
                throw new StoredProcedureExecutionException(ex.Message);
            }
        }
    }
}
