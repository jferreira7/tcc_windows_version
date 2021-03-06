﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using tcc_windows_version.Model;
using tcc_windows_version.View;

namespace tcc_windows_version.Database
{
    class DespesasCRUD
    {
        Connection objConexao = new Connection();
        MySqlCommand objComando = new MySqlCommand();
        MySqlDataAdapter adpt;
        DataTable data;
        public void Create(Despesas despesa)
        {
            objComando.CommandType = CommandType.Text;
            objComando.CommandText = "insert into despesas (estado, nome, empresa, categoria, valor, data_vencimento, id_usuario) VALUES (@estado, @nome, @empresa, @categoria, @valor, @data_vencimento, @id_usuario);";

            objComando.Parameters.Add("@estado", MySqlDbType.VarChar, 8).Value = despesa.estado;
            objComando.Parameters.Add("@nome", MySqlDbType.VarChar, 150).Value = despesa.nome;
            objComando.Parameters.Add("@empresa", MySqlDbType.VarChar, 150).Value = despesa.empresa;
            objComando.Parameters.Add("@categoria", MySqlDbType.VarChar, 100).Value = despesa.categoria;
            objComando.Parameters.Add("@valor", MySqlDbType.Decimal, 12).Value = despesa.valor;
            objComando.Parameters.Add("@data_vencimento", MySqlDbType.Date).Value = despesa.data_vencimento;
            objComando.Parameters.Add("@id_usuario", MySqlDbType.Int32).Value = despesa.id_usuario;

            try
            {
                objConexao.Conexao();
                objComando.Connection = objConexao.Conectar();
                objComando.ExecuteNonQuery();
                objConexao.Desconectar();
                Mensagem.mensagemSucesso = "Despesa adicionada com sucesso!";
            }
            catch //(Exception erro)
            {
                Mensagem.mensagemErro = "Erro de conexão com o servidor! Tente mais tarde.";
            }        
        }
        public DataView Read(int id_usuario)
        {
            try
            {
                data = new DataTable("despesas");
                string anoAtual = DateTime.Now.Year.ToString();
                adpt = new MySqlDataAdapter("select * from despesas where id_usuario = '" + id_usuario + "' and YEAR(data_vencimento) = " + anoAtual + " order by data_vencimento desc;", objConexao.Conexao());
                adpt.Fill(data);
                objConexao.Desconectar();
                return data.DefaultView;
            }
            catch //(Exception erro)
            {                
                Mensagem.mensagemErro = "Erro de conexão com o servidor! Tente mais tarde.";
                return null;
            }
        }
        public void Update(Despesas despesa)
        {
            objComando.CommandText = "update despesas set estado = @estado, empresa = @empresa, nome = @nome, categoria = @categoria, valor = @valor, data_vencimento = @data_vencimento where id = @id and id_usuario = @id_usuario;";
            
            objComando.Parameters.Add("@estado", MySqlDbType.VarChar, 8).Value = despesa.estado;
            objComando.Parameters.Add("@nome", MySqlDbType.VarChar, 150).Value = despesa.nome;
            objComando.Parameters.Add("@empresa", MySqlDbType.VarChar, 150).Value = despesa.empresa;
            objComando.Parameters.Add("@categoria", MySqlDbType.VarChar, 100).Value = despesa.categoria;
            objComando.Parameters.Add("@valor", MySqlDbType.Decimal, 12).Value = Convert.ToDecimal(despesa.valor);
            objComando.Parameters.Add("@data_vencimento", MySqlDbType.Date).Value = despesa.data_vencimento;
            objComando.Parameters.Add("@id", MySqlDbType.Int32).Value = Convert.ToInt32(despesa.id);
            objComando.Parameters.Add("@id_usuario", MySqlDbType.Int32).Value = Convert.ToInt32(despesa.id_usuario);

            try
            {
                objConexao.Conexao();
                objComando.Connection = objConexao.Conectar();
                objComando.ExecuteNonQuery();
                objConexao.Desconectar();
                Mensagem.mensagemSucesso = "Despesa atualizada com sucesso!";
            }
            catch //(Exception erro)
            {
                Mensagem.mensagemErro = "Erro de conexão com o servidor! Tente mais tarde.";
            }
        }
        public void Delete(int id)
        {
            objComando.CommandText = "delete from despesas where id = @id";
            objComando.Parameters.Add("@id", MySqlDbType.Int32, 11).Value = id;

            try
            {
                objConexao.Conexao();
                objComando.Connection = objConexao.Conectar();
                objComando.ExecuteNonQuery();
                objConexao.Desconectar();
                Mensagem.mensagemSucesso = "Despesa deletada com sucesso!";
            }
            catch //(Exception erro)
            {
                Mensagem.mensagemErro = "Erro de conexão com o servidor! Tente mais tarde.";
            }
        }
        public DataView Search(string nome, string empresa, string categoria, string mes, string ano, string estado, int id_usuario)
        {
            string query = "select * from despesas where ";            
            
            /*"select* from despesas where nome like '%nada%' and empresa like '%DAAE%' and categoria like '%men%' and MONTH(data_vencimento) = '10' and YEAR(data_vencimento) = '2020' and estado = 'Pendente';"*/
            if (nome != "") { query += "nome like '%" + nome + "%' and "; }
            if (empresa != "") { query += "empresa like '%" + empresa + "%' and "; }
            if (categoria != "") { query += "categoria like '%" + categoria + "%' and "; }
            if (mes != "") { query += "MONTH(data_vencimento) = '" + mes + "' and "; }
            if (ano != "") { query += "YEAR(data_vencimento) = '" + ano + "' and "; }
            if (estado != "") { query += "estado = '" + estado + "' and "; }
            if (id_usuario > 0) { query += "id_usuario = '" + id_usuario.ToString() + "' and "; }

            query = query.TrimEnd(' ', 'a', 'n', 'd', ' ');
            query += ";";            
            
            data = new DataTable("despesas");

            try
            {
                adpt = new MySqlDataAdapter(query, objConexao.Conexao());
                adpt.Fill(data);
                objConexao.Desconectar();
                return data.DefaultView;
            }
            catch //(Exception erro)
            {
                Mensagem.mensagemErro = "Erro de conexão com o servidor! Tente mais tarde.";
                return null;
            }
        }
        public void UpdateStatus(int id)
        {
            objComando.CommandText = "UPDATE despesas SET estado = 'Atrasado' WHERE data_vencimento < current_date() AND estado = 'Pendente' AND id_usuario = @id";
            objComando.Parameters.Add("@id", MySqlDbType.Int32, 11).Value = id;

            try
            {
                objConexao.Conexao();
                objComando.Connection = objConexao.Conectar();
                objComando.ExecuteNonQuery();
                objConexao.Desconectar();
                Mensagem.mensagemSucesso = "Despesa deletada com sucesso!";
            }
            catch //(Exception erro)
            {
                Mensagem.mensagemErro = "Erro de conexão com o servidor! Tente mais tarde.";
            }
        }
    }
}
