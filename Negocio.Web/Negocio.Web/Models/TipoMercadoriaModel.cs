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
    public class TipoMercadoriaModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o tipo de Mercadoria.")]

        public string Tipo { get; set; }

        public bool Ativo { get; set; }

        public static List<TipoMercadoriaModel> RecuperarLista()
        {
            var ret = new List<TipoMercadoriaModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from tipo_mercadoria order by tipo";
                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        ret.Add(new TipoMercadoriaModel
                        {
                            Id = (int)reader["id"],
                            Tipo = (string)reader["tipo"],
                            Ativo = (bool)reader["ativo"]
                        });
                    }
                }
            }

            return ret;
        }

        public static TipoMercadoriaModel RecuperarPeloId(int id)
        {
            TipoMercadoriaModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from tipo_mercadoria where (id = @id)";

                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        ret = new TipoMercadoriaModel
                        {
                            Id = (int)reader["id"],
                            Tipo = (string)reader["tipo"],
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
                        comando.CommandText = "delete from tipo_mercadoria where (id = @id)";

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
                        comando.CommandText = "insert into tipo_mercadoria (tipo, ativo) values (@tipo, @ativo); select convert(int, scope_identity())";

                        comando.Parameters.Add("@tipo", SqlDbType.VarChar).Value = this.Tipo;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);

                        ret = (int)comando.ExecuteScalar();
                    }
                    else
                    {
                        comando.CommandText = "update tipo_mercadoria set tipo=@tipo, ativo=@ativo where id = @id";

                        comando.Parameters.Add("@tipo", SqlDbType.VarChar).Value = this.Tipo;
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