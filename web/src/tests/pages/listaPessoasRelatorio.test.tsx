import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { BrowserRouter } from "react-router-dom";
import { ListaPessoasRelatorio } from "../../pages/ListaPessoasRelatorio";
import api from "../../services/api";

vi.mock("../../services/api");
const mockedApi = vi.mocked(api);

const mockNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return { ...actual, useNavigate: () => mockNavigate };
});

describe("ListaPessoasRelatorio Page", () => {
  const mockPessoas = [
    { id: "1", nome: "Rodrigo", idade: 30 },
    { id: "2", nome: "Ana", idade: 25 },
  ];

  const mockTransacoes = [
    { id: "t1", pessoaId: "1", valor: 1000, tipo: 2 }, // Receita Rodrigo
    { id: "t2", pessoaId: "1", valor: 400, tipo: 1 }, // Despesa Rodrigo
    { id: "t3", pessoaId: "2", valor: 500, tipo: 2 }, // Receita Ana
  ];

  beforeEach(() => {
    vi.clearAllMocks();
    mockedApi.get.mockImplementation((url) => {
      if (url === "/Pessoas") return Promise.resolve({ data: mockPessoas });
      if (url === "/Transacoes")
        return Promise.resolve({ data: mockTransacoes });
      return Promise.resolve({ data: [] });
    });
  });

  const renderPage = () =>
    render(<ListaPessoasRelatorio />, { wrapper: BrowserRouter });

  it("Deve_Carregar_E_Calcular_Totais_Individuais_Corretamente", async () => {
    renderPage();

    const nomePessoa = await screen.findByText(/RODRIGO/i);
    const card = nomePessoa.closest(".group");

    if (!card) throw new Error("Card da pessoa não encontrado");

    const { getByText } = within(card as HTMLElement);

    expect(getByText("R$ 1000.00")).toBeInTheDocument();
    expect(getByText("R$ 400.00")).toBeInTheDocument();
    expect(getByText("R$ 600.00")).toBeInTheDocument();
  });

  it("Deve_Calcular_Resumo_Geral_Do_Sistema", async () => {
    renderPage();

    expect(
      await screen.findByText(/Resumo Geral do Sistema/i),
    ).toBeInTheDocument();

    expect(screen.getByText("R$ 1500.00")).toBeInTheDocument(); // Total Receitas (1000 + 500)
    expect(
      screen.getByText("R$ 400.00", { selector: ".text-red-400" }),
    ).toBeInTheDocument(); // Total Despesas
    expect(screen.getByText("R$ 1100.00")).toBeInTheDocument(); // Saldo Geral (1500 - 400)
  });

  it("Deve_Navegar_Para_Detalhes_Ao_Clicar_No_Card", async () => {
    renderPage();

    const card = await screen.findByText(/RODRIGO/i);
    await userEvent.click(card);

    expect(mockNavigate).toHaveBeenCalledWith("/relatorios/pessoa/1");
  });

  it("Deve_Navegar_De_Volta_Para_Menu_De_Totais", async () => {
    renderPage();

    const botaoVoltar = screen.getByRole("button", { name: "←" });
    await userEvent.click(botaoVoltar);

    expect(mockNavigate).toHaveBeenCalledWith("/totais");
  });

  it("Deve_Exibir_Mensagem_Caso_Nao_Haja_Pessoas", async () => {
    mockedApi.get.mockResolvedValue({ data: [] });
    renderPage();

    expect(
      await screen.findByText(/Nenhuma pessoa cadastrada para exibir totais/i),
    ).toBeInTheDocument();
  });
});
