import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { BrowserRouter } from "react-router-dom";
import { DetalhePessoa } from "../../pages/DetalhePessoa";
import api from "../../services/api";

vi.mock("../../services/api");
const mockedApi = vi.mocked(api);

const mockNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return {
    ...actual,
    useNavigate: () => mockNavigate,
    useParams: () => ({ id: "1" }),
  };
});

describe("DetalhePessoa Page", () => {
  const mockPessoa = { id: "1", nome: "Rodrigo", idade: 30 };
  const mockTransacoes = [
    { id: "t1", pessoaId: "1", descricao: "Salário", valor: 5000, tipo: 2 },
    { id: "t2", pessoaId: "1", descricao: "Aluguel", valor: 1200, tipo: 1 },
    {
      id: "t3",
      pessoaId: "99",
      descricao: "Outra Pessoa",
      valor: 100,
      tipo: 1,
    },
  ];

  beforeEach(() => {
    vi.clearAllMocks();
    mockedApi.get.mockImplementation((url) => {
      if (url.includes("/Pessoas/1"))
        return Promise.resolve({ data: mockPessoa });
      if (url === "/Transacoes")
        return Promise.resolve({ data: mockTransacoes });
      return Promise.resolve({ data: {} });
    });
  });

  const renderPage = () =>
    render(<DetalhePessoa />, { wrapper: BrowserRouter });

  it("Deve_Exibir_Estado_De_Carregamento_Inicial", () => {
    renderPage();
    expect(screen.getByText(/Carregando.../i)).toBeInTheDocument();
  });

  it("Deve_Renderizar_Perfil_E_Filtrar_Transacoes_Corretamente", async () => {
    renderPage();

    expect(await screen.findByText(/rodrigo/i)).toBeInTheDocument();

    // Verifica se a transação da outra pessoa foi filtrada
    expect(screen.queryByText("Outra Pessoa")).not.toBeInTheDocument();
    expect(screen.getByText("Salário")).toBeInTheDocument();
  });

  it("Deve_Calcular_Totais_Do_Cabecalho_Corretamente", async () => {
    renderPage();

    // Localiza o container de totais pela classe
    const containerTotais = await findByClassName("card-detalhes");
    const { getByText } = within(containerTotais);

    // Receita: 5000, Despesa: 1200, Saldo: 3800
    expect(getByText("R$ 5000.00")).toBeInTheDocument();
    expect(getByText("R$ 1200.00")).toBeInTheDocument();
    expect(getByText("R$ 3800.00")).toBeInTheDocument();
  });

  it("Deve_Navegar_Ao_Clicar_No_Botao_Voltar", async () => {
    renderPage();
    const botaoVoltar = await screen.findByText(/Voltar/i);
    await userEvent.click(botaoVoltar);

    expect(mockNavigate).toHaveBeenCalledWith(-1);
  });
});

// Helper para o Vitest encontrar por classe já que getByClassName não é nativo do screen
async function findByClassName(className: string) {
  return await screen.findByText(
    (_, element) => element?.classList.contains(className) ?? false,
  );
}
