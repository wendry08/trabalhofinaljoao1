namespace AtividadeFuncionario_we
{
    public class Funcionario
{
    public string Nome { get; set; }
    public string CPF { get; set; }
    public string CTPS { get; set; }
    public string RG { get; set; }
    public string Funcao { get; set; }
    public string Setor { get; set; }
    public string Sala { get; set; }
    public string Telefone { get; set; }
    public Endereco Endereco { get; set; }
}

public class Endereco
{
    public string UF { get; set; }
    public string Cidade { get; set; }
    public string Bairro { get; set; }
    public string Numero { get; set; }
    public string CEP { get; set; }
}

}
