import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { BrowserRouter } from "react-router-dom";
import { DetalheCategoria } from "../../pages/DetalheCategoria";
import api from "../../services/api";

vi.mock("../../services/api");
const mockedApi = vi.mocked(api);

const mockNavigate = vi.fn();

vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return {
    ...actual,
    useNavigate: () => mockNavigate,
    useParams: () => ({ id: "cat-123" }),
  };
});

// Helper local para encontrar por classe
async function findByClassName(className: string) {
  return await screen.findByText(
    (_, element) => element?.classList.contains(className) ?? false,
  );
}

describe("DetalheCategoria Page", () => {
  const mockCategoria = {
    id: "cat-123",
    descricao: "Alimentação",
    finalidade: "Despesa",
  };

  const mockTransacoes = [
    {
      id: "t1",
      categoriaId: "cat-123",
      descricao: "Mercado",
      valor: 250.5,
      tipo: 1,
      pessoaNome: "Rodrigo",
    },
    {
      id: "t2",
      categoriaId: "cat-123",
      descricao: "Estorno",
      valor: 50.0,
      tipo: 2,
      pessoaNome: "Rodrigo",
    },
    {
      id: "t3",
      categoriaId: "OUTRA",
      descricao: "Gasolina",
      valor: 100,
      tipo: 1,
      pessoaNome: "Ana",
    },
  ];

  beforeEach(() => {
    vi.clearAllMocks();
    mockedApi.get.mockImplementation((url) => {
      if (url.includes("/Categorias/cat-123"))
        return Promise.resolve({ data: mockCategoria });
      if (url === "/Transacoes")
        return Promise.resolve({ data: mockTransacoes });
      return Promise.resolve({ data: {} });
    });
  });

  const renderPage = () =>
    render(<DetalheCategoria />, { wrapper: BrowserRouter });

  it("Deve_Exibir_Carregamento_E_Depois_Descricao_Da_Categoria", async () => {
    renderPage();
    expect(screen.getByText(/Carregando categoria/i)).toBeInTheDocument();

    expect(await screen.findByText(/ALIMENTAÇÃO/i)).toBeInTheDocument();
  });

  it("Deve_Filtrar_Transacoes_E_Calcular_Totais_Corretamente", async () => {
    renderPage();

    const containerTotais = await findByClassName("card-detalhes");
    const { getByText } = within(containerTotais);

    expect(getByText("R$ 50.00")).toBeInTheDocument();
    expect(getByText("R$ 250.50")).toBeInTheDocument();
    expect(getByText("R$ -200.50")).toBeInTheDocument();

    expect(screen.getByText(/Mercado/i)).toBeInTheDocument();
    expect(screen.queryByText(/Gasolina/i)).not.toBeInTheDocument();
  });

  it("Deve_Navegar_De_Volta_Ao_Clicar_No_Botao", async () => {
    renderPage();
    const botaoVoltar = await screen.findByText(/Voltar/i);
    await userEvent.click(botaoVoltar);

    expect(mockNavigate).toHaveBeenCalledWith(-1);
  });

  it("Deve_Exibir_Estado_Vazio_Quando_Sem_Movimentacoes", async () => {
    // Mock de transações vazias para este teste específico
    mockedApi.get.mockImplementation((url) => {
      if (url.includes("/Categorias/"))
        return Promise.resolve({ data: mockCategoria });
      return Promise.resolve({ data: [] });
    });

    renderPage();
    expect(
      await screen.findByText(/Nenhum registro para esta categoria/i),
    ).toBeInTheDocument();
  });
});
