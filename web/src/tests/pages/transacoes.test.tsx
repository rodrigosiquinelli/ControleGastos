import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { BrowserRouter } from "react-router-dom";
import { Transacoes } from "../../pages/Transacoes";
import api from "../../services/api";

vi.mock("../../services/api");
const mockedApi = vi.mocked(api);

describe("Transacoes Page", () => {
  const mockPessoas = [
    { id: "p1", nome: "João Menor", idade: 15 },
    { id: "p2", nome: "Rodrigo Adulto", idade: 30 },
  ];
  const mockCategorias = [
    { id: "c1", descricao: "Aluguel", finalidade: "Despesa" },
    { id: "c2", descricao: "Salário", finalidade: "Receita" },
  ];

  beforeEach(() => {
    vi.clearAllMocks();
    mockedApi.get.mockImplementation((url) => {
      if (url === "/Transacoes") return Promise.resolve({ data: [] });
      if (url === "/Pessoas") return Promise.resolve({ data: mockPessoas });
      if (url === "/Categorias")
        return Promise.resolve({ data: mockCategorias });
      return Promise.resolve({ data: [] });
    });
  });

  const renderPage = () => render(<Transacoes />, { wrapper: BrowserRouter });

  it("Deve_Validar_Valor_Maior_Que_Zero", async () => {
    const alertSpy = vi.spyOn(window, "alert").mockImplementation(() => {});
    renderPage();

    // Preenche tudo para passar pelo 'required' do HTML, mas coloca valor 0
    await userEvent.type(screen.getByLabelText(/Descrição/i), "Teste");
    await userEvent.type(screen.getByLabelText(/Valor/i), "0");
    await userEvent.selectOptions(screen.getByLabelText(/Responsável/i), "p1");
    await userEvent.selectOptions(screen.getByLabelText(/Categoria/i), "c1");

    await userEvent.click(screen.getByRole("button", { name: /FINALIZAR/i }));

    expect(alertSpy).toHaveBeenCalledWith(
      expect.stringContaining("maior que zero"),
    );
  });

  it("Deve_Bloquear_Receita_Para_Menores_De_18_Anos", async () => {
    const alertSpy = vi.spyOn(window, "alert").mockImplementation(() => {});
    renderPage();

    await userEvent.type(screen.getByLabelText(/Descrição/i), "Mesada");
    await userEvent.type(screen.getByLabelText(/Valor/i), "100");
    await userEvent.selectOptions(screen.getByLabelText(/Tipo/i), "Receita");
    await userEvent.selectOptions(screen.getByLabelText(/Responsável/i), "p1"); // João Menor
    await userEvent.selectOptions(screen.getByLabelText(/Categoria/i), "c2"); // Receita

    await userEvent.click(screen.getByRole("button", { name: /FINALIZAR/i }));

    expect(alertSpy).toHaveBeenCalledWith(
      "🚫 Menores de 18 anos não podem registrar Receitas!",
    );
  });

  it("Deve_Validar_Se_Categoria_Eh_Exclusiva_Para_Receitas", async () => {
    const alertSpy = vi.spyOn(window, "alert").mockImplementation(() => {});
    renderPage();

    await userEvent.type(screen.getByLabelText(/Descrição/i), "Pagamento");
    await userEvent.type(screen.getByLabelText(/Valor/i), "50");
    await userEvent.selectOptions(screen.getByLabelText(/Tipo/i), "Despesa");
    await userEvent.selectOptions(screen.getByLabelText(/Responsável/i), "p2");
    await userEvent.selectOptions(screen.getByLabelText(/Categoria/i), "c2"); // Receita)

    await userEvent.click(screen.getByRole("button", { name: /FINALIZAR/i }));

    expect(alertSpy).toHaveBeenCalledWith(
      expect.stringContaining("exclusiva para RECEITAS"),
    );
  });

  it("Deve_Registrar_Transacao_Com_Sucesso", async () => {
    const alertSpy = vi.spyOn(window, "alert").mockImplementation(() => {});
    mockedApi.post.mockResolvedValueOnce({});
    renderPage();

    await userEvent.type(screen.getByLabelText(/Descrição/i), "Aluguel Mensal");
    await userEvent.type(screen.getByLabelText(/Valor/i), "1500");
    await userEvent.selectOptions(screen.getByLabelText(/Tipo/i), "Despesa");
    await userEvent.selectOptions(screen.getByLabelText(/Responsável/i), "p2");
    await userEvent.selectOptions(screen.getByLabelText(/Categoria/i), "c1");

    await userEvent.click(screen.getByRole("button", { name: /FINALIZAR/i }));

    await waitFor(() => {
      expect(mockedApi.post).toHaveBeenCalled();
      expect(alertSpy).toHaveBeenCalledWith(expect.stringContaining("sucesso"));
    });
  });
});
