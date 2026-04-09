import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, vi } from "vitest";
import { BrowserRouter } from "react-router-dom";
import { Totais } from "../../pages/Totais";

const mockNavigate = vi.fn();

vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  };
});

describe("Totais Page", () => {
  const renderPage = () => render(<Totais />, { wrapper: BrowserRouter });

  it("Deve_Navegar_Para_Relatorio_De_Pessoas", async () => {
    renderPage();

    const botaoPessoa = screen.getByRole("button", { name: /Por Pessoa/i });
    await userEvent.click(botaoPessoa);

    expect(mockNavigate).toHaveBeenCalledWith("/relatorios/lista-pessoas");
  });

  it("Deve_Navegar_Para_Relatorio_De_Categorias", async () => {
    renderPage();

    const botaoCategoria = screen.getByRole("button", {
      name: /Por Categoria/i,
    });
    await userEvent.click(botaoCategoria);

    expect(mockNavigate).toHaveBeenCalledWith("/relatorios/lista-categorias");
  });
});
