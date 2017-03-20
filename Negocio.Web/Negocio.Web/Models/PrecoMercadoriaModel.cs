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
    public class PrecoMercadoriaModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preço da mercadoria.")]
        public string Preco { get; set; }

        public bool Ativo { get; set; }

        public static List<PrecoMercadoriaModel> RecuperarLista()
        {
            var ret = new List<PrecoMercadoriaModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from preco_mercadoria order by preco";
                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        ret.Add(new PrecoMercadoriaModel
                        {
                            Id = (int)reader["id"],
                            Preco = (string)reader["preco"],
                            Ativo = (bool)reader["ativo"]
                        });
                    }
                }
            }

            return ret;
        }

        public static PrecoMercadoriaModel RecuperarPeloId(int id)
        {
            PrecoMercadoriaModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from preco_mercadoria where (id = @id)";

                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        ret = new PrecoMercadoriaModel
                        {
                            Id = (int)reader["id"],
                            Preco = (string)reader["preco"],
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
                        comando.CommandText = "delete from preco_mercadoria where (id = @id)";

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
                        comando.CommandText = "insert into preco_mercadoria (preco,ativo) values (@preco,@ativo); select convert(int, scope_identity())";

                        comando.Parameters.Add("@preco", SqlDbType.VarChar).Value = this.Preco;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);

                        ret = (int)comando.ExecuteScalar();
                    }
                    else
                    {
                        comando.CommandText = "update preco_mercadoria set preco=@preco,ativo=@ativo where id = @id";

                        comando.Parameters.Add("@preco", SqlDbType.VarChar).Value = this.Preco;
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