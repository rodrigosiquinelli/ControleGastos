import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import { usePagination } from "../hooks/usePagination";

interface Categoria {
  id: string;
  descricao: string;
  finalidade: any;
}

// Componente para gerenciamento de categorias.
export function Categorias() {
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [descricao, setDescricao] = useState("");
  const [finalidade, setFinalidade] = useState("Despesa");
  const [carregando, setCarregando] = useState(true);
  const [editandoId, setEditandoId] = useState<string | null>(null);

  const navigate = useNavigate();

  const { currentItems, currentPage, totalPages, goToNext, goToPrev } =
    usePagination(categorias, 10);

  // Inicializa a listagem de categorias ao carregar o componente.
  useEffect(() => {
    carregarCategorias();
  }, []);

  // Consome a API para buscar todas as categorias cadastradas e atualiza o estado local.
  async function carregarCategorias() {
    try {
      setCarregando(true);
      const res = await api.get("/Categorias");
      setCategorias(Array.isArray(res.data) ? res.data : []);
    } catch (err) {
      console.error("Erro ao buscar categorias:", err);
    } finally {
      setCarregando(false);
    }
  }

  // Prepara o formulário para edição.
  function prepararEdicao(cat: Categoria, e: React.MouseEvent) {
    e.stopPropagation();
    setEditandoId(cat.id);
    setDescricao(cat.descricao);

    const fin = Number(cat.finalidade);
    setFinalidade(fin === 2 ? "Receita" : fin === 3 ? "Ambas" : "Despesa");

    window.scrollTo({ top: 0, behavior: "smooth" });
  }

  // Reseta o estado do formulário e limpa o ID de edição.
  function cancelarEdicao() {
    setEditandoId(null);
    setDescricao("");
    setFinalidade("Despesa");
  }

  // Envia os dados para salvar ou atualizar a categoria.
  async function salvar(e: React.FormEvent) {
    e.preventDefault();
    if (!descricao.trim()) return alert("Digite uma descrição!");

    const finalidadeNumerica =
      finalidade === "Receita" ? 2 : finalidade === "Ambas" ? 3 : 1;

    try {
      const payload = { descricao, finalidade: finalidadeNumerica };

      if (editandoId) {
        await api.put(`/Categorias/${editandoId}`, {
          ...payload,
          id: editandoId,
        });
        alert("✅ Categoria atualizada!");
      } else {
        await api.post("/Categorias", payload);
        alert("✅ Categoria salva!");
      }

      cancelarEdicao();
      carregarCategorias();
    } catch (error: any) {
      const apiErro =
        error.response?.data?.Detailed ||
        error.response?.data?.Message ||
        "Erro ao salvar.";
      alert(`❌ ${apiErro}`);
    }
  }

  return (
    <div className="max-w-5xl mx-auto p-4 space-y-8 animate-in fade-in duration-500 text-left">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 border-l-4 border-green-500 pl-4">
        <div>
          <h1 className="text-3xl font-black text-gray-800">
            {editandoId ? "Editando Categoria" : "Categorias"}
          </h1>
          <p className="text-gray-500">
            Defina os tipos de receitas e despesas
          </p>
        </div>
        <span className="bg-green-100 text-green-700 px-4 py-1 rounded-full text-sm font-bold">
          {categorias.length} Registradas
        </span>
      </div>

      {/* Formulário de criação e atualização de categorias */}
      <form
        onSubmit={salvar}
        className={`bg-white p-8 rounded-3xl shadow-xl border-2 transition-all ${editandoId ? "border-green-500" : "border-transparent"} grid grid-cols-1 md:grid-cols-3 gap-6 items-end`}
      >
        <div className="md:col-span-1 flex flex-col gap-2 text-left">
          <label
            htmlFor="descricao"
            className="text-sm font-bold text-gray-700 ml-1"
          >
            Descrição
          </label>
          <input
            id="descricao"
            className="border-2 border-gray-100 rounded-2xl p-4 focus:border-green-500 outline-none transition-all bg-gray-50/50 focus:bg-white"
            value={descricao}
            onChange={(e) => setDescricao(e.target.value)}
            maxLength={400}
            required
          />
        </div>

        <div className="flex flex-col gap-2 text-left">
          <label
            htmlFor="finalidade"
            className="text-sm font-bold text-gray-700 ml-1"
          >
            Finalidade
          </label>
          <select
            id="finalidade"
            className="border-2 border-gray-100 rounded-2xl p-4 bg-gray-50/50 outline-none focus:border-green-500 cursor-pointer"
            value={finalidade}
            onChange={(e) => setFinalidade(e.target.value)}
          >
            <option value="Despesa">🔴 Despesa</option>
            <option value="Receita">🟢 Receita</option>
            <option value="Ambas">🔵 Ambas</option>
          </select>
        </div>

        <div className="flex gap-2">
          {editandoId && (
            <button
              type="button"
              onClick={cancelarEdicao}
              className="bg-gray-100 text-gray-600 font-bold py-5 px-6 rounded-2xl hover:bg-gray-200 transition-all"
            >
              X
            </button>
          )}
          <button
            className={`${editandoId ? "bg-orange-500 hover:bg-orange-600" : "bg-green-600 hover:bg-green-700"} text-white font-black py-5 px-8 rounded-2xl transition-all active:scale-95 flex-1`}
          >
            {editandoId ? "SALVAR" : "ADICIONAR"}
          </button>
        </div>
      </form>

      {carregando ? (
        <p className="text-center py-20 text-gray-400 font-medium italic animate-pulse">
          Carregando categorias...
        </p>
      ) : (
        <>
          {/* Listagem de categorias renderizada em formato de cards interativos */}
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
            {currentItems.map((cat) => {
              const fin = Number(cat.finalidade);
              return (
                <div
                  key={cat.id}
                  onClick={() => navigate(`/relatorios/categoria/${cat.id}`)}
                  className={`bg-white p-6 rounded-[2rem] shadow-sm border-2 flex justify-between items-center group transition-all cursor-pointer ${editandoId === cat.id ? "border-green-500 scale-[1.02]" : "border-gray-100 hover:border-green-400 hover:shadow-xl"}`}
                >
                  <div>
                    <p className="font-black text-gray-800 text-lg group-hover:text-green-600 transition-colors uppercase tracking-tight">
                      {cat.descricao}
                    </p>
                    <div className="flex items-center gap-2 mt-1">
                      <span
                        className={`w-2 h-2 rounded-full ${
                          fin === 2
                            ? "bg-green-500"
                            : fin === 1
                              ? "bg-red-500"
                              : "bg-blue-500"
                        }`}
                      ></span>
                      <p className="text-gray-400 text-[10px] font-black uppercase tracking-widest">
                        {fin === 2
                          ? "Receita"
                          : fin === 1
                            ? "Despesa"
                            : "Ambas"}
                      </p>
                    </div>
                  </div>
                  <div className="flex items-center gap-2">
                    <button
                      onClick={(e) => prepararEdicao(cat, e)}
                      className="p-2 rounded-lg bg-gray-50 text-gray-400 opacity-0 group-hover:opacity-100 hover:bg-green-500 hover:text-white transition-all"
                    >
                      ✏️
                    </button>
                    <span className="text-gray-200 group-hover:text-green-500 group-hover:translate-x-1 transition-all text-xl">
                      ➔
                    </span>
                  </div>
                </div>
              );
            })}
          </div>

          {/* Controles de Navegação da Paginação */}
          {totalPages > 1 && (
            <div className="flex flex-col md:flex-row items-center justify-between gap-4 pt-4 border-t border-gray-100">
              <p className="text-sm font-bold text-gray-400 uppercase">
                Página {currentPage} de {totalPages}
              </p>
              <div className="flex gap-2">
                <button
                  onClick={goToPrev}
                  disabled={currentPage === 1}
                  className="px-6 py-2 rounded-xl bg-white border-2 border-gray-100 font-black text-gray-600 hover:bg-gray-50 disabled:opacity-30 transition-all"
                >
                  Anterior
                </button>
                <button
                  onClick={goToNext}
                  disabled={currentPage === totalPages}
                  className="px-6 py-2 rounded-xl bg-white border-2 border-gray-100 font-black text-gray-600 hover:bg-gray-50 disabled:opacity-30 transition-all"
                >
                  Próxima
                </button>
              </div>
            </div>
          )}
        </>
      )}
    </div>
  );
}
