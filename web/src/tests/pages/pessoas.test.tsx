import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { BrowserRouter } from "react-router-dom";
import { Pessoas } from "../../pages/Pessoas";
import api from "../../services/api";

vi.mock("../../services/api");
const mockedApi = vi.mocked(api);

const mockNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return { ...actual, useNavigate: () => mockNavigate };
});

describe("Pessoas Page", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockedApi.get.mockResolvedValue({ data: [] });
  });

  const renderPage = () => render(<Pessoas />, { wrapper: BrowserRouter });

  it("Deve_Carregar_E_Listar_Pessoas_Ao_Montar_Componente", async () => {
    const pessoasMock = [
      {
        id: "1",
        nome: "Thiago",
        idade: 30,
        dataNascimento: "1994-01-01T00:00:00Z",
      },
    ];
    mockedApi.get.mockResolvedValueOnce({ data: pessoasMock });

    renderPage();

    expect(await screen.findByText(/Thiago/i)).toBeInTheDocument();
    expect(screen.getByText(/30 ANOS/i)).toBeInTheDocument();
  });

  it("Deve_Exibir_Alerta_Se_Nome_For_Curto_Ao_Salvar", async () => {
    const alertSpy = vi.spyOn(window, "alert").mockImplementation(() => {});
    renderPage();

    const inputNome = screen.getByLabelText(/Nome Completo/i);
    const inputData = screen.getByLabelText(/Data de Nascimento/i);
    const botaoSalvar = screen.getByRole("button", { name: /CADASTRAR/i });

    await userEvent.type(inputNome, "Ro");
    await userEvent.type(inputData, "2000-01-01");
    await userEvent.click(botaoSalvar);

    expect(alertSpy).toHaveBeenCalledWith("Digite o nome completo.");
    expect(mockedApi.post).not.toHaveBeenCalled();
  });

  it("Deve_Cadastrar_Nova_Pessoa_Com_Sucesso", async () => {
    const alertSpy = vi.spyOn(window, "alert").mockImplementation(() => {});
    mockedApi.post.mockResolvedValueOnce({});
    renderPage();

    await userEvent.type(
      screen.getByLabelText(/Nome Completo/i),
      "Novo Usuario",
    );
    await userEvent.type(
      screen.getByLabelText(/Data de Nascimento/i),
      "2000-01-01",
    );
    await userEvent.click(screen.getByRole("button", { name: /CADASTRAR/i }));

    await waitFor(() => {
      expect(mockedApi.post).toHaveBeenCalled();
      expect(alertSpy).toHaveBeenCalledWith(
        expect.stringContaining("Cadastrado com sucesso"),
      );
    });
  });

  it("Deve_Entrar_No_Modo_Edicao_Ao_Clicar_Em_Editar", async () => {
    const pessoa = {
      id: "1",
      nome: "Teste",
      idade: 20,
      dataNascimento: "2004-01-01T00:00:00Z",
    };
    mockedApi.get.mockResolvedValueOnce({ data: [pessoa] });

    renderPage();

    const botaoEditar = await screen.findByText("✏️");
    await userEvent.click(botaoEditar);

    expect(screen.getByText(/Editando Pessoa/i)).toBeInTheDocument();
    expect(screen.getByDisplayValue("Teste")).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /CANCELAR/i }),
    ).toBeInTheDocument();
  });

  it("Deve_Chamar_Exclusao_Apos_Confirmacao", async () => {
    const pessoa = { id: "1", nome: "Excluir", idade: 20 };
    mockedApi.get.mockResolvedValueOnce({ data: [pessoa] });
    vi.spyOn(window, "confirm").mockReturnValue(true);

    renderPage();

    const botaoExcluir = await screen.findByText("🗑️");
    await userEvent.click(botaoExcluir);

    expect(mockedApi.delete).toHaveBeenCalledWith("/Pessoas/1");
  });
});
