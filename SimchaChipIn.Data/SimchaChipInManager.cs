using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimchaChipIn.Data
{
    public class SimchaChipInManager
    {
        private readonly string connectionString;

        public SimchaChipInManager(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public void AddSimcha(Simcha simcha)
        {
            
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"INSERT INTO Simchas(Name,Date)
                                      VALUES(@name,@date);
                                      SELECT SCOPE_IDENTITY()";
                command.Parameters.AddWithValue("@name", simcha.Name);
                command.Parameters.AddWithValue("@date", simcha.Date);
                connection.Open();
                simcha.Id= (int)(decimal)command.ExecuteScalar();
            }
        }
        public IEnumerable<Simcha> GetSimchas()
        {
            var simchas = new List<Simcha>();
            using (var connection = new SqlConnection(connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT *, (
	                            SELECT ISNull(SUM(Amount), 0)
                                            FROM Contributions
                                            WHERE SimchaId = s.Id 
                            ) as 'Total', (
                            SELECT COUNT(*)
                                            FROM Contributions
                                            WHERE SimchaId = s.Id 
                            ) as 'ContributorAmount' FROM Simchas s";
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var simcha = new Simcha();
                    simcha.Id = (int)reader["Id"];
                    simcha.Date = (DateTime)reader["Date"];
                    simcha.Name = (string)reader["Name"];
                    simcha.ContributorAmount = (int)reader["ContributorAmount"];
                    simcha.Total = (decimal)reader["Total"];
                    simchas.Add(simcha);
                }
            }

            return simchas;
        }
        public Simcha GetSimchaById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT *, (
	                    SELECT ISNull(SUM(Amount), 0)
                                    FROM Contributions
                                    WHERE SimchaId = s.Id 
                    ) as 'Total', (
                    SELECT COUNT(*)
                                    FROM Contributions
                                    WHERE SimchaId = s.Id 
                    ) as 'ContributorAmount' FROM Simchas s
                    WHERE Id = @simchaId";
                command.Parameters.AddWithValue("@simchaId", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return new Simcha
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    ContributorAmount = (int)reader["ContributorAmount"],
                    Total = (decimal)reader["Total"]

                };
            }
        }
        public void AddContributor(Contributor contributor)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {

                command.CommandText = @"INSERT INTO Contributors(FirstName, LastName, CellNumber, Date, AlwaysInclude)
                                        VALUES(@firstName,@lastName,@cellNumber,@date,@alwaysInclude) SELECT SCOPE_IDENTITY()";
                connection.Open();
                command.Parameters.AddWithValue("@firstName", contributor.FirstName);
                command.Parameters.AddWithValue("@lastName", contributor.LastName);
                command.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
                command.Parameters.AddWithValue("@date", contributor.Date);
                command.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
                contributor.Id = (int)(decimal)command.ExecuteScalar();

            }
        }
        public int GetContributorCount()
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM Contributors";
                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }
        public IEnumerable<Contributor> GetContributors()
        {
            var contributors = new List<Contributor>();
            using (var connection = new SqlConnection(connectionString))
            using (var cmd = connection.CreateCommand())
            {
                connection.Open();
                cmd.CommandText = @"SELECT *, 
(
	(SELECT ISNULL(SUM(d.Amount), 0)
            FROM Deposits d WHERE d.ContributorId = c.Id)
	- 
	(SELECT ISNULL(SUM(co.Amount), 0)
            FROM Contributions co WHERE co.ContributorId = c.Id)
)
as 'Balance' FROM Contributors c";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var contributor = new Contributor();
                    contributor.Id = (int)reader["Id"];
                    contributor.FirstName = (string)reader["FirstName"];
                    contributor.LastName = (string)reader["LastName"];
                    contributor.CellNumber = (string)reader["CellNumber"];
                    contributor.Date = (DateTime)reader["Date"];
                    contributor.AlwaysInclude = (bool)reader["AlwaysInclude"];
                    contributor.Balance = (decimal)reader["Balance"];
                    contributors.Add(contributor);
                }
            }

            return contributors;
        }
        public IEnumerable<SimchaContributor> GetSimchaContributors(int simchaId)
        {
            IEnumerable<Contributor> contributors = GetContributors();
            using (var connection = new SqlConnection(connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Contributions WHERE SimchaId = @simchaId";
                cmd.Parameters.AddWithValue("@simchaId", simchaId);
                connection.Open();
                var reader = cmd.ExecuteReader();
                List<Contribution> contributions = new List<Contribution>();
                while (reader.Read())
                {
                    Contribution contribution = new Contribution
                    {
                        Amount = (decimal)reader["Amount"],
                        SimchaId = simchaId,
                        ContributorId = (int)reader["ContributorId"]
                    };
                    contributions.Add(contribution);
                }

                return contributors.Select(contributor =>
                {
                    var sc = new SimchaContributor();
                    sc.FirstName = contributor.FirstName;
                    sc.LastName = contributor.LastName;
                    sc.AlwaysInclude = contributor.AlwaysInclude;
                    sc.ContributorId = contributor.Id;
                    sc.Balance = contributor.Balance;
                    Contribution contribution = contributions.FirstOrDefault(c => c.ContributorId == contributor.Id);
                    if (contribution != null)
                    {
                        sc.Amount = contribution.Amount;
                    }
                    return sc;
                });
            }

        }
        public void UpdateContributor(Contributor contributor)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"UPDATE Contributors SET FirstName = @firstName, LastName = @lastName,
                                        CellNumber = @cellNumber, Date = @date, AlwaysInclude = @alwaysInclude WHERE Id = @id";
                connection.Open();
                command.Parameters.AddWithValue("@firstName", contributor.FirstName);
                command.Parameters.AddWithValue("@lastName", contributor.LastName);
                command.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
                command.Parameters.AddWithValue("@date", contributor.Date);
                command.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
                command.Parameters.AddWithValue("@id", contributor.Id);
                command.ExecuteNonQuery();

            }
        }


        public void UpdateSimchaContributions(int simchaId, IEnumerable<ContributionInclusion> contributors)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Contributions WHERE SimchaId = @simchaId";
                cmd.Parameters.AddWithValue("@simchaId", simchaId);

                connection.Open();
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                cmd.CommandText = @"INSERT INTO Contributions (SimchaId, ContributorId, Amount)
                                    VALUES (@simchaId, @contributorId, @amount)";
                foreach (ContributionInclusion contributor in contributors.Where(c => c.Include))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@simchaId", simchaId);
                    cmd.Parameters.AddWithValue("@contributorId", contributor.ContributorId);
                    cmd.Parameters.AddWithValue("@amount", contributor.Amount);
                    cmd.ExecuteNonQuery();
                }

            }

        }
        public IEnumerable<Contribution> GetContributionsById(int contributorId)
        {
            List<Contribution> contributions = new List<Contribution>();
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT c.*, s.Name, s.Date FROM Contributions c
                                        JOIN Simchas s ON c.SimchaId=s.Id
                                        WHERE ContributorId = @contributorId";
                command.Parameters.AddWithValue("@contributorId", contributorId);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var contribution = new Contribution()
                    {
                        ContributorId = (int)reader["ContributorId"],
                        Amount = (decimal)reader["Amount"],
                        SimchaId = (int)reader["SimchaId"],
                        SimchaName = (string)reader["Name"],
                        Date = (DateTime)reader["Date"]
                    };
                    contributions.Add(contribution);
                }
            }
            return contributions;
        }
        public void AddDeposit(Deposit deposit)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"INSERT INTO Deposits (Date, Amount, ContributorId)
                                     VALUES (@date, @amount, @contributorId)";
                command.Parameters.AddWithValue("@date", deposit.Date);
                command.Parameters.AddWithValue("@amount", deposit.Amount);
                command.Parameters.AddWithValue("@contributorId", deposit.ContributorId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public IEnumerable<Deposit> GetDepositsById(int contributorId)
        {
            List<Deposit> deposits = new List<Deposit>();
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM Deposits WHERE ContributorId=@contributorId";
                command.Parameters.AddWithValue("@contributorId", contributorId);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Deposit deposit = new Deposit()
                    {
                        Id=(int)reader["Id"],
                        Date = (DateTime)reader["Date"],
                        Amount = (decimal)reader["Amount"]   
                    };
                    deposits.Add(deposit);
                }
            }
            return deposits;
        }
    }
}

