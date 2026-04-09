import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import api from "../services/api";
import { usePagination } from "../hooks/usePagination";

// Componente de detalhamento individual que exibe o perfil da pessoa e seu extrato financeiro.
export function DetalhePessoa() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [pessoa, setPessoa] = useState<any>(null);
  const [transacoes, setTransacoes] = useState<any[]>([]);

  // Configuração do hook de paginação.
  const { currentItems, currentPage, totalPages, goToNext, goToPrev } =
    usePagination(transacoes, 10);

  // Carrega os dados do perfil e a lista de transações.
  useEffect(() => {
    async function carregar() {
      try {
        const [resP, resT] = await Promise.all([
          api.get(`/Pessoas/${id}`),
          api.get(`/Transacoes`),
        ]);

        setPessoa(resP.data);

        // Filtra apenas as transações pertencentes a esta pessoa.
        const filtradas = (resT.data || [])
          .filter((t: any) => t.pessoaId === id)
          .map((t: any) => ({
            ...t,
            valor: Number(t.valor),
          }));

        setTransacoes(filtradas);
      } catch (err) {
        console.error("Erro ao carregar detalhes:", err);
      }
    }
    carregar();
  }, [id]);

  // Exibe uma mensagem de carregamento enquanto os dados da API não são retornados.
  if (!pessoa)
    return (
      <div className="p-20 text-center font-bold text-gray-400">
        Carregando...
      </div>
    );

  // Realiza o somatório total de receitas e despesas.
  const totalReceita = transacoes
    .filter((t) => t.tipo === 2)
    .reduce((acc, t) => acc + t.valor, 0);

  const totalDespesa = transacoes
    .filter((t) => t.tipo === 1)
    .reduce((acc, t) => acc + t.valor, 0);

  const saldo = totalReceita - totalDespesa;

  return (
    <div className="max-w-4xl mx-auto space-y-6 text-left animate-in fade-in duration-500">
      <button
        onClick={() => navigate(-1)}
        className="font-bold text-blue-600 hover:underline"
      >
        ← Voltar
      </button>

      {/* Cabeçalho de perfil com os totais financeiros da pessoa. */}
      <div className="bg-white p-10 rounded-[2.5rem] shadow-xl shadow-gray-200/50 border border-gray-100">
        <div className="flex flex-col md:flex-row justify-between items-center gap-8">
          <div>
            <h1 className="text-4xl font-black text-gray-900 uppercase tracking-tighter">
              {pessoa.nome}
            </h1>
            <p className="text-gray-400 font-bold mt-1 uppercase text-xs">
              Idade: {pessoa.idade} anos
            </p>
          </div>

          <div className="card-detalhes p-6 rounded-[2rem] flex gap-8 border border-gray-100">
            <div className="text-center">
              <p className="text-[10px] font-black text-gray-400 uppercase">
                Receitas
              </p>
              <p className="text-xl font-black text-green-600">
                R$ {totalReceita.toFixed(2)}
              </p>
            </div>
            <div className="text-center border-x px-8">
              <p className="text-[10px] font-black text-gray-400 uppercase">
                Despesas
              </p>
              <p className="text-xl font-black text-red-600">
                R$ {totalDespesa.toFixed(2)}
              </p>
            </div>
            <div className="text-center">
              <p className="text-[10px] font-black text-gray-400 uppercase">
                Saldo
              </p>
              <p
                className={`text-xl font-black ${saldo >= 0 ? "text-blue-600" : "text-red-600"}`}
              >
                R$ {saldo.toFixed(2)}
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Tabela de histórico de transacoes. */}
      <div className="bg-white rounded-[2rem] shadow-sm border border-gray-100 overflow-hidden">
        <table className="w-full">
          <tbody className="divide-y divide-gray-50">
            {currentItems.map((t) => (
              <tr key={t.id} className="hover:bg-gray-50/50 transition-colors">
                <td className="px-8 py-6">
                  <p className="font-bold text-gray-800 text-lg">
                    {t.descricao}
                  </p>
                  <span
                    className={`text-[10px] font-black uppercase ${t.tipo === 2 ? "text-green-500" : "text-red-500"}`}
                  >
                    {t.tipo === 2 ? "Receita" : "Despesa"}
                  </span>
                </td>
                <td
                  className={`px-8 py-6 text-right font-black text-xl ${t.tipo === 2 ? "text-green-600" : "text-red-600"}`}
                >
                  {t.tipo === 2 ? "+" : "-"} R$ {t.valor.toFixed(2)}
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        {/* Controles de Navegação da Paginação */}
        {totalPages > 1 && (
          <div className="p-6 bg-gray-50/30 border-t flex items-center justify-between">
            <span className="text-xs font-black text-gray-400 uppercase">
              Página {currentPage} de {totalPages}
            </span>
            <div className="flex gap-2">
              <button
                onClick={goToPrev}
                disabled={currentPage === 1}
                className="px-6 py-2 rounded-xl bg-white border-2 border-gray-100 font-black text-gray-600 hover:bg-gray-50 disabled:opacity-30 transition-all shadow-sm"
              >
                Anterior
              </button>
              <button
                onClick={goToNext}
                disabled={currentPage === totalPages}
                className="px-6 py-2 rounded-xl bg-white border-2 border-gray-100 font-black text-gray-600 hover:bg-gray-50 disabled:opacity-30 transition-all shadow-sm"
              >
                Próxima
              </button>
            </div>
          </div>
        )}

        {/* Estado vazio para quando não há registros retornados pela API */}
        {transacoes.length === 0 && (
          <p className="p-20 text-center text-gray-400 italic">
            Nenhuma movimentação para esta pessoa.
          </p>
        )}
      </div>
    </div>
  );
}
