using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace Negocio.Web.Models
{
    public class QuantidadeMercadoriaModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a quantidade de mercadoria vendida.")]
        public string Quantidade { get; set; }

        public bool Ativo { get; set; }

        public static List<QuantidadeMercadoriaModel> RecuperarLista()
        {
            var ret = new List<QuantidadeMercadoriaModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from quantidade_mercadoria order by quantidade";
                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        ret.Add(new QuantidadeMercadoriaModel
                        {
                            Id = (int)reader["id"],
                            Quantidade = (string)reader["quantidade"],
                            Ativo = (bool)reader["ativo"]
                        });
                    }
                }
            }

            return ret;
        }

        public static QuantidadeMercadoriaModel RecuperarPeloId(int id)
        {
            QuantidadeMercadoriaModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from quantidade_mercadoria where (id = @id)";

                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        ret = new QuantidadeMercadoriaModel
                        {
                            Id = (int)reader["id"],
                            Quantidade = (string)reader["quantidade"],
                            Ativo = (bool)reader["ativo"]
                        };
                    }
                }
            }

            return ret;
        }

        public static bool ExcluirPeloId(int id)
        {
            var ret = false;

            if (RecuperarPeloId(id) != null)
            {
                using (var conexao = new SqlConnection())
                {
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                    conexao.Open();
                    using (var comando = new SqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.CommandText = "delete from quantidade_mercadoria where (id = @id)";

                        comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                        ret = (comando.ExecuteNonQuery() > 0);
                    }
                }
            }

            return ret;
        }

        public int Salvar()
        {
            var ret = 0;

            var model = RecuperarPeloId(this.Id);

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;

                    if (model == null)
                    {
                        comando.CommandText = "insert into quantidade_mercadoria (quantidade,ativo) values (@quantidade,@ativo); select convert(int, scope_identity())";

                        comando.Parameters.Add("@quantidade", SqlDbType.VarChar).Value = this.Quantidade;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);

                        ret = (int)comando.ExecuteScalar();
                    }
                    else
                    {
                        comando.CommandText = "update quantidade_mercadoria set quantidade=@quantidade,ativo=@ativo where id = @id";

                        comando.Parameters.Add("@quantidade", SqlDbType.VarChar).Value = this.Quantidade;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);
                        comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;

                        if (comando.ExecuteNonQuery() > 0)
                        {
                            ret = this.Id;
                        }
                    }
                }
            }

            return ret;
        }
    }
}