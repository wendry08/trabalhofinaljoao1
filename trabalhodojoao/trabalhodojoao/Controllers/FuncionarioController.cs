using AtividadeFuncionario_we;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Funcionario_we.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuncionarioController : ControllerBase
    {
        private const string ArquivoFuncionarios = "funcionarios.txt";

        private bool VerificarCPF(string cpf)
        {
            cpf = Regex.Replace(cpf, @"\D", "");
            if (cpf.Length != 11 || cpf.All(c => c == cpf[0]))
            {
                return false;
            }

            int[] PrimeiroDigito = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] SegundoDigito = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            int soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * PrimeiroDigito[i];
            }
            int resto = soma % 11;
            string digito1 = resto < 2 ? "0" : (11 - resto).ToString();

            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * SegundoDigito[i];
            }
            resto = soma % 11;
            string digito2 = resto < 2 ? "0" : (11 - resto).ToString();

            return cpf.EndsWith(digito1 + digito2);
        }

        private List<Funcionario> ObterFuncionarios()
        {
            var funcionarios = new List<Funcionario>();

            if (System.IO.File.Exists(ArquivoFuncionarios))
            {
                var linhas = System.IO.File.ReadAllLines(ArquivoFuncionarios);
                foreach (var linha in linhas)
                {
                    var dados = linha.Split('|');
                    if (dados.Length == 13)
                    {
                        funcionarios.Add(new Funcionario
                        {
                            Nome = dados[0],
                            CPF = dados[1],
                            CTPS = dados[2],
                            RG = dados[3],
                            Funcao = dados[4],
                            Setor = dados[5],
                            Sala = dados[6],
                            Telefone = dados[7],
                            Endereco = new Endereco
                            {
                                UF = dados[8],
                                Cidade = dados[9],
                                Bairro = dados[10],
                                Numero = dados[11],
                                CEP = dados[12]
                            }
                        });
                    }
                    else
                    {
                        Console.WriteLine("erro: " + linha);
                    }
                }
            }
            else
            {
                Console.WriteLine("arquivo nao existente: " + ArquivoFuncionarios);
            }

            return funcionarios;
        }

        private void SalvarFuncionarios(List<Funcionario> funcionarios)
        {
            var linhas = funcionarios.Select(f => $"{f.Nome}|{f.CPF}|{f.CTPS}|{f.RG}|{f.Funcao}|{f.Setor}|{f.Sala}|{f.Telefone}|{f.Endereco.UF}|{f.Endereco.Cidade}|{f.Endereco.Bairro}|{f.Endereco.Numero}|{f.Endereco.CEP}");
            System.IO.File.WriteAllLines(ArquivoFuncionarios, linhas);
        }

        [HttpGet]
        public IActionResult ObterTodos()
        {
            var funcionarios = ObterFuncionarios();
            return Ok(funcionarios);
        }

        [HttpGet("{cpf}")]
        public IActionResult ObterPorCPF(string cpf)
        {
            if (!VerificarCPF(cpf)) //validacao

            {
                return BadRequest("cpf nao encontrado.");
            }

            var funcionarios = ObterFuncionarios();
            var funcionario = funcionarios.FirstOrDefault(f => f.CPF == cpf);

            if (funcionario == null)
            {
                return NotFound();
            }

            return Ok(funcionario);
        }

        [HttpPost]
        public IActionResult Adicionar([FromBody] funcionarioDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("dados invalidos.");
            }

            if (!VerificarCPF(dto.CPF))
            {
                return BadRequest("cpf invalido.");
            }

            var funcionarios = ObterFuncionarios();
            if (funcionarios.Any(f => f.CPF == dto.CPF))
            {
                return Conflict("ja existe um funcionario com esse cpf, digite outro.");
            }

            var funcionario = new Funcionario
            {
                Nome = dto.Nome,
                CPF = dto.CPF,
                CTPS = dto.CTPS,
                RG = dto.RG,
                Funcao = dto.Funcao,
                Setor = dto.Setor,
                Sala = dto.Sala,
                Telefone = dto.Telefone,
                Endereco = new Endereco
                {
                    UF = dto.Endereco.UF,
                    Cidade = dto.Endereco.Cidade,
                    Bairro = dto.Endereco.Bairro,
                    Numero = dto.Endereco.Numero,
                    CEP = dto.Endereco.CEP
                }
            };

            funcionarios.Add(funcionario);
            SalvarFuncionarios(funcionarios);

            return CreatedAtAction(nameof(ObterPorCPF), new { cpf = funcionario.CPF }, funcionario);
        }

        [HttpPut("{cpf}")]
        public IActionResult Atualizar(string cpf, [FromBody] funcionarioDTO dto)
        {
            if (!VerificarCPF(cpf))
            {
                return BadRequest("CPF inválido.");
            }

            var funcionarios = ObterFuncionarios();
            var funcionario = funcionarios.FirstOrDefault(f => f.CPF == cpf);

            if (funcionario == null)
            {
                return NotFound();
            }

            funcionario.Nome = dto.Nome ?? funcionario.Nome;
            funcionario.CTPS = dto.CTPS ?? funcionario.CTPS;
            funcionario.RG = dto.RG ?? funcionario.RG;
            funcionario.Funcao = dto.Funcao ?? funcionario.Funcao;
            funcionario.Setor = dto.Setor ?? funcionario.Setor;
            funcionario.Sala = dto.Sala ?? funcionario.Sala;
            funcionario.Telefone = dto.Telefone ?? funcionario.Telefone;
            funcionario.Endereco.UF = dto.Endereco.UF ?? funcionario.Endereco.UF;
            funcionario.Endereco.Cidade = dto.Endereco.Cidade ?? funcionario.Endereco.Cidade;
            funcionario.Endereco.Bairro = dto.Endereco.Bairro ?? funcionario.Endereco.Bairro;
            funcionario.Endereco.Numero = dto.Endereco.Numero ?? funcionario.Endereco.Numero;
            funcionario.Endereco.CEP = dto.Endereco.CEP ?? funcionario.Endereco.CEP;

            SalvarFuncionarios(funcionarios);

            return Ok(funcionario);
        }

        [HttpDelete("{cpf}")]
        public IActionResult Remover(string cpf)
        {
            if (!VerificarCPF(cpf))
            {
                return BadRequest("CPF inválido.");
            }

            var funcionarios = ObterFuncionarios();
            var funcionario = funcionarios.FirstOrDefault(f => f.CPF == cpf);

            if (funcionario == null)
            {
                return NotFound();
            }

            funcionarios.Remove(funcionario);
            SalvarFuncionarios(funcionarios);

            return Ok(funcionario);
        }
    }
}

