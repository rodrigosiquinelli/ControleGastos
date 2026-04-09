import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { BrowserRouter } from "react-router-dom";
import { ListaCategoriasRelatorio } from "../../pages/ListaCategoriasRelatorio";
import api from "../../services/api";

vi.mock("../../services/api");
const mockedApi = vi.mocked(api);

const mockNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return { ...actual, useNavigate: () => mockNavigate };
});

describe("ListaCategoriasRelatorio Page", () => {
  const mockCategorias = [
    { id: "c1", descricao: "Alimentação", finalidade: 1 }, // Despesa
    { id: "c2", descricao: "Investimentos", finalidade: 3 }, // Ambas
  ];

  const mockTransacoes = [
    { id: "t1", categoriaId: "c1", valor: 200, tipo: 1 }, // Despesa Alimentação
    { id: "t2", categoriaId: "c2", valor: 500, tipo: 2 }, // Receita Investimento
    { id: "t3", categoriaId: "c2", valor: 100, tipo: 1 }, // Despesa Investimento
  ];

  beforeEach(() => {
    vi.clearAllMocks();
    mockedApi.get.mockImplementation((url) => {
      if (url === "/Categorias")
        return Promise.resolve({ data: mockCategorias });
      if (url === "/Transacoes")
        return Promise.resolve({ data: mockTransacoes });
      return Promise.resolve({ data: [] });
    });
  });

  const renderPage = () =>
    render(<ListaCategoriasRelatorio />, { wrapper: BrowserRouter });

  it("Deve_Calcular_Totais_Por_Categoria_Corretamente", async () => {
    renderPage();

    // Localiza o card de Investimentos
    const nomeCat = await screen.findByText(/INVESTIMENTOS/i);
    const card = nomeCat.closest(".group");
    if (!card) throw new Error("Card não encontrado");

    const { getByText } = within(card as HTMLElement);

    // Receita: 500, Despesa: 100, Saldo: 400
    expect(getByText("R$ 500.00")).toBeInTheDocument();
    expect(getByText("R$ 100.00")).toBeInTheDocument();
    expect(getByText("R$ 400.00")).toBeInTheDocument();
  });

  it("Deve_Exibir_Balanco_Geral_Das_Categorias", async () => {
    renderPage();

    // Localiza o container de Balanço Geral
    const containerGeral = (
      await screen.findByText(/Balanço Geral por Categorias/i)
    ).closest("div");
    if (!containerGeral) throw new Error("Resumo geral não encontrado");

    const { getByText } = within(containerGeral as HTMLElement);

    // Total Receita: 500, Total Despesa: 300 (200 + 100), Saldo: 200
    expect(getByText("R$ 500.00")).toBeInTheDocument();
    expect(getByText("R$ 300.00")).toBeInTheDocument();
    expect(getByText("R$ 200.00")).toBeInTheDocument();
  });

  it("Deve_Navegar_Para_Detalhes_Da_Categoria_Ao_Clicar", async () => {
    renderPage();
    const card = await screen.findByText(/ALIMENTAÇÃO/i);
    await userEvent.click(card);

    expect(mockNavigate).toHaveBeenCalledWith("/relatorios/categoria/c1");
  });

  it("Deve_Navegar_De_Volta_Para_Totais", async () => {
    renderPage();
    const botaoVoltar = screen.getByRole("button", { name: "←" });
    await userEvent.click(botaoVoltar);

    expect(mockNavigate).toHaveBeenCalledWith("/totais");
  });

  it("Deve_Exibir_Estado_Vazio_Quando_Sem_Categorias", async () => {
    mockedApi.get.mockResolvedValueOnce({ data: [] });
    renderPage();

    expect(
      await screen.findByText(/Nenhuma categoria registrada/i),
    ).toBeInTheDocument();
  });
});
