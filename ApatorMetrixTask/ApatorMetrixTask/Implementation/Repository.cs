using ApatorMetrixTask.Interfaces;
using ApatorMetrixTask.Models;
using MySql.Data.MySqlClient;

namespace ApatorMetrixTask.Implementation
{
    public class Repository : IRepository
    {
        private const string connectionString = "server=localhost;uid=root;pwd=<haslo_do_bazy_danych>;database=apatormetrix;Pooling=true;Min Pool Size=10;Max Pool Size=500;";

        public async Task<bool> AddNewPaymentCardAsync(PaymentCard paymentCard)
        {
            try
            {
                using (var mySqlConnection = new MySqlConnection(connectionString))
                {
                    await mySqlConnection.OpenAsync();

                    using (MySqlCommand mySqlCommand = new MySqlCommand())
                    {
                        mySqlCommand.Connection = mySqlConnection;

                        MySqlTransaction transaction = await mySqlConnection.BeginTransactionAsync();

                        mySqlCommand.Transaction = transaction;

                        mySqlCommand.CommandText = "INSERT INTO Payment_Card(Owner_Account_Number,Pin,Card_Serial_Number,UCID) " +
                                                   "VALUES(@OwnerAccountNumber,@Pin,@CardSerialNumber,@UCID)";

                        mySqlCommand.Parameters.AddWithValue("@OwnerAccountNumber", paymentCard.OwnerAccountNumber);
                        mySqlCommand.Parameters.AddWithValue("@Pin", paymentCard.Pin);
                        mySqlCommand.Parameters.AddWithValue("@CardSerialNumber", paymentCard.CardSerialNumber);
                        mySqlCommand.Parameters.AddWithValue("@UCID", paymentCard.UCID);

                        if (await mySqlCommand.ExecuteNonQueryAsync() == 1)
                        {
                            await transaction.CommitAsync();
                            return true;
                        }

                        await transaction.RollbackAsync();
                    }
                }
            }
            catch (Exception e)
            {
            }

            return false;
        }

        public async Task<List<PaymentCard>> FindPaymentCardAsync(PaymentCard paymentCard)
        {
            var paymentCards = new List<PaymentCard>();
            try
            {
                using (var mySqlConnection = new MySqlConnection(connectionString))
                {
                    await mySqlConnection.OpenAsync();

                    using (MySqlCommand mySqlCommand = new MySqlCommand())
                    {
                        mySqlCommand.Connection = mySqlConnection;

                        string whereCondition = "";

                        if (!paymentCard.OwnerAccountNumber.Equals("")) whereCondition += " Owner_Account_Number = @OwnerAccountNumber ";

                        if(!paymentCard.CardSerialNumber.Equals(""))
                        {
                            if (!whereCondition.Equals("")) whereCondition += " AND Card_Serial_Number = @CardSerialNumber ";
                            else whereCondition += " Card_Serial_Number = @CardSerialNumber ";
                        }

                        if (!paymentCard.UCID.Equals(""))
                        {
                            if (!whereCondition.Equals("")) whereCondition += " AND UCID = @UCID ";
                            else whereCondition += " UCID = @UCID ";
                        }

                        mySqlCommand.CommandText = $"SELECT * FROM Payment_Card WHERE {whereCondition}";

                        mySqlCommand.Parameters.AddWithValue("@OwnerAccountNumber", paymentCard.OwnerAccountNumber);
                        mySqlCommand.Parameters.AddWithValue("@CardSerialNumber", paymentCard.CardSerialNumber);
                        mySqlCommand.Parameters.AddWithValue("@UCID", paymentCard.UCID);

                        using(var reader = await mySqlCommand.ExecuteReaderAsync())
                        {
                            while(await reader.ReadAsync())
                            {
                                paymentCards.Add(new PaymentCard
                                {
                                    PaymentCardID = reader.GetInt32(0),
                                    OwnerAccountNumber = reader.GetString(1),
                                    Pin = reader.GetString(2),
                                    CardSerialNumber = reader.GetString(3),
                                    UCID = reader.GetString(4),
                                    CardNumber = reader.GetString(5),
                                    ExpiryDate = reader.GetDateTime(6).ToString("yyyy-MM-dd"),
                                    CVV = reader.GetString(7),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }

            return paymentCards;
        }

        public async Task<List<PaymentCard>> GetAllPaymentCardsAsync()
        {
            var paymentCards = new List<PaymentCard>();
            try
            {
                using (var mySqlConnection = new MySqlConnection(connectionString))
                {
                    await mySqlConnection.OpenAsync();

                    using (MySqlCommand mySqlCommand = new MySqlCommand())
                    {
                        mySqlCommand.Connection = mySqlConnection;

                        mySqlCommand.CommandText = "SELECT * FROM Payment_Card";

                        using (var reader = await mySqlCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                paymentCards.Add(new PaymentCard
                                {
                                    PaymentCardID = reader.GetInt32(0),
                                    OwnerAccountNumber = reader.GetString(1),
                                    Pin = reader.GetString(2),
                                    CardSerialNumber = reader.GetString(3),
                                    UCID = reader.GetString(4),
                                    CardNumber = reader.GetString(5),
                                    ExpiryDate = reader.GetDateTime(6).ToString("yyyy-MM-dd"),
                                    CVV = reader.GetString(7),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }

            return paymentCards;
        }

        public async Task<bool> RemovePaymentCardAsync(PaymentCard paymentCard)
        {
            try
            {
                using (var mySqlConnection = new MySqlConnection(connectionString))
                {
                    await mySqlConnection.OpenAsync();

                    using (MySqlCommand mySqlCommand = new MySqlCommand())
                    {
                        mySqlCommand.Connection = mySqlConnection;

                        MySqlTransaction transaction = await mySqlConnection.BeginTransactionAsync();

                        mySqlCommand.Transaction = transaction;

                        mySqlCommand.CommandText = "SELECT Payment_Card_ID FROM Payment_Card WHERE " +
                                                   "Owner_Account_Number = @OwnerAccountNumber AND " +
                                                   "Card_Serial_Number = @CardSerialNumber AND " +
                                                   "UCID = @UCID";

                        mySqlCommand.Parameters.AddWithValue("@OwnerAccountNumber", paymentCard.OwnerAccountNumber);
                        mySqlCommand.Parameters.AddWithValue("@CardSerialNumber", paymentCard.CardSerialNumber);
                        mySqlCommand.Parameters.AddWithValue("@UCID", paymentCard.UCID);

                        var id = await mySqlCommand.ExecuteScalarAsync();

                        if (id is not null)
                        {
                            id = id.ToString();

                            mySqlCommand.CommandText = "DELETE FROM Payment_Card WHERE Payment_Card_ID = @PaymentCardID";
                            mySqlCommand.Parameters.AddWithValue("@PaymentCardID", id);

                            if (await mySqlCommand.ExecuteNonQueryAsync() == 1)
                            {
                                await transaction.CommitAsync();
                                return true;
                            }

                            await transaction.RollbackAsync();
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }

            return false;
        }

        public async Task<bool> RemovePaymentFromListCardAsync(PaymentCard paymentCard)
        {
            try
            {
                using (var mySqlConnection = new MySqlConnection(connectionString))
                {
                    await mySqlConnection.OpenAsync();

                    using (MySqlCommand mySqlCommand = new MySqlCommand())
                    {
                        mySqlCommand.Connection = mySqlConnection;

                        MySqlTransaction transaction = await mySqlConnection.BeginTransactionAsync();

                        mySqlCommand.Transaction = transaction;

                        mySqlCommand.CommandText = "SELECT Payment_Card_ID FROM Payment_Card WHERE " +
                                                   "Owner_Account_Number = @OwnerAccountNumber AND " +
                                                   "Card_Serial_Number = @CardSerialNumber AND " +
                                                   "UCID = @UCID";

                        mySqlCommand.Parameters.AddWithValue("@OwnerAccountNumber", paymentCard.OwnerAccountNumber);
                        mySqlCommand.Parameters.AddWithValue("@CardSerialNumber", paymentCard.CardSerialNumber);
                        mySqlCommand.Parameters.AddWithValue("@UCID", paymentCard.UCID);

                        var id = await mySqlCommand.ExecuteScalarAsync();

                        if (id is not null)
                        {
                            id = id.ToString();

                            mySqlCommand.CommandText = "DELETE FROM Payment_Card WHERE Payment_Card_ID = @PaymentCardID";
                            mySqlCommand.Parameters.AddWithValue("@PaymentCardID", id);

                            if (await mySqlCommand.ExecuteNonQueryAsync() == 1)
                            {
                                await transaction.CommitAsync();
                                return true;
                            }

                            await transaction.RollbackAsync();
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }

            return false;
        }

        public async Task<string> GetPaymentCardHolderAsync(int paymentCardID)
        {
            string cardHolderFullName = "";

            try
            {
                using (var mySqlConnection = new MySqlConnection(connectionString))
                {
                    await mySqlConnection.OpenAsync();

                    using (MySqlCommand mySqlCommand = new MySqlCommand())
                    {
                        mySqlCommand.Connection = mySqlConnection;

                        mySqlCommand.CommandText = "SELECT Name,Surname FROM Payment_Card_Owner WHERE Payment_Card_ID = @PaymentCardID";

                        mySqlCommand.Parameters.AddWithValue("@PaymentCardID", paymentCardID);

                        using (var reader = await mySqlCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                cardHolderFullName = $"{reader.GetString(0)} {reader.GetString(1)}";
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }

            return cardHolderFullName;
        }
    }
}
