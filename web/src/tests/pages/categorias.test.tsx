import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { BrowserRouter } from "react-router-dom";
import { Categorias } from "../../pages/Categorias";
import api from "../../services/api";

// Mocks
vi.mock("../../services/api");
const mockedApi = vi.mocked(api);

const mockNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return { ...actual, useNavigate: () => mockNavigate };
});

describe("Categorias Page", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockedApi.get.mockResolvedValue({ data: [] });
  });

  const renderPage = () => render(<Categorias />, { wrapper: BrowserRouter });

  it("Deve_Exibir_Mensagem_De_Carregamento_Inicial", () => {
    renderPage();
    expect(screen.getByText(/Carregando categorias/i)).toBeInTheDocument();
  });

  it("Deve_Listar_Categorias_Com_Sucesso", async () => {
    const categoriasMock = [
      { id: "1", descricao: "Alimentação", finalidade: 1 },
      { id: "2", descricao: "Salário", finalidade: 2 },
    ];
    mockedApi.get.mockResolvedValueOnce({ data: categoriasMock });

    renderPage();

    expect(await screen.findByText(/ALIMENTAÇÃO/i)).toBeInTheDocument();
    expect(screen.getByText(/SALÁRIO/i)).toBeInTheDocument();
    expect(screen.getByText(/2 Registradas/i)).toBeInTheDocument();
  });

  it("Deve_Validar_Descricao_Vazia_Ao_Salvar", async () => {
    const alertSpy = vi.spyOn(window, "alert").mockImplementation(() => {});
    renderPage();

    const botaoAdicionar = await screen.findByRole("button", {
      name: /ADICIONAR/i,
    });

    await userEvent.click(botaoAdicionar);

    const input = screen.getByLabelText(/Descrição/i);
    await userEvent.type(input, "   ");
    await userEvent.click(botaoAdicionar);

    expect(alertSpy).toHaveBeenCalledWith("Digite uma descrição!");
  });

  it("Deve_Cadastrar_Nova_Categoria_Com_Sucesso", async () => {
    const alertSpy = vi.spyOn(window, "alert").mockImplementation(() => {});
    mockedApi.post.mockResolvedValueOnce({});
    renderPage();

    await userEvent.type(screen.getByLabelText(/Descrição/i), "Combustível");
    await userEvent.selectOptions(
      screen.getByLabelText(/Finalidade/i),
      "Despesa",
    );
    await userEvent.click(screen.getByRole("button", { name: /ADICIONAR/i }));

    await waitFor(() => {
      expect(mockedApi.post).toHaveBeenCalledWith("/Categorias", {
        descricao: "Combustível",
        finalidade: 1,
      });
      expect(alertSpy).toHaveBeenCalledWith(
        expect.stringContaining("Categoria salva"),
      );
    });
  });

  it("Deve_Carregar_Dados_No_Formulario_Ao_Editar", async () => {
    const categoria = { id: "123", descricao: "Lazer", finalidade: 3 }; // Ambas
    mockedApi.get.mockResolvedValueOnce({ data: [categoria] });

    renderPage();

    const botaoEditar = await screen.findByText("✏️");
    await userEvent.click(botaoEditar);

    expect(screen.getByDisplayValue("Lazer")).toBeInTheDocument();
    expect(screen.getByLabelText(/Finalidade/i)).toHaveValue("Ambas");
    expect(screen.getByText(/Editando Categoria/i)).toBeInTheDocument();
  });

  it("Deve_Navegar_Para_Detalhes_Ao_Clicar_No_Card", async () => {
    const categoria = { id: "99", descricao: "Saúde", finalidade: 1 };
    mockedApi.get.mockResolvedValueOnce({ data: [categoria] });

    renderPage();

    const card = await screen.findByText(/SAÚDE/i);
    await userEvent.click(card);

    expect(mockNavigate).toHaveBeenCalledWith("/relatorios/categoria/99");
  });
});
