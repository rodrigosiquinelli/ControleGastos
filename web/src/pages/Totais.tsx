import { useNavigate } from "react-router-dom";

// Componente de menu de relatórios para navegação entre visões por Pessoa ou por Categoria.
export function Totais() {
  const navigate = useNavigate();

  return (
    <div className="max-w-5xl mx-auto space-y-12 animate-in fade-in duration-700">
      <div className="text-left">
        <h1 className="text-4xl font-black text-gray-900 tracking-tight">
          Relatórios e Consultas
        </h1>
        <p className="text-gray-500 text-lg mt-2">
          Selecione uma categoria para visualizar o detalhamento financeiro.
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        {/* Opção de navegação para o relatório detalhado agrupado por pessoas. */}
        <button
          onClick={() => navigate("/relatorios/lista-pessoas")}
          className="group bg-white p-10 rounded-[2.5rem] shadow-xl shadow-blue-100/40 border-2 border-transparent hover:border-blue-500 transition-all text-left flex flex-col justify-between min-h-[300px]"
        >
          <div className="bg-blue-50 w-20 h-20 rounded-3xl flex items-center justify-center text-4xl group-hover:scale-110 transition-transform">
            👥
          </div>
          <div>
            <h2 className="text-3xl font-black text-gray-800 tracking-tighter">
              Por Pessoa
            </h2>
            <p className="text-gray-400 mt-4 leading-relaxed">
              Consulte o extrato individual, data de nascimento e o saldo
              líquido de cada morador.
            </p>
          </div>
          <span className="text-blue-600 font-bold mt-6 flex items-center gap-2">
            Acessar Relatório{" "}
            <span className="group-hover:translate-x-2 transition-transform">
              →
            </span>
          </span>
        </button>

        {/* Opção de navegação para o relatório detalhado agrupado por categorias. */}
        <button
          onClick={() => navigate("/relatorios/lista-categorias")}
          className="group bg-white p-10 rounded-[2.5rem] shadow-xl shadow-gray-100/40 border-2 border-transparent hover:border-blue-500 transition-all text-left flex flex-col justify-between min-h-[300px]"
        >
          <div className="bg-gray-50 w-20 h-20 rounded-3xl flex items-center justify-center text-4xl group-hover:scale-110 transition-transform">
            📂
          </div>
          <div>
            <h2 className="text-3xl font-black text-gray-800 tracking-tighter">
              Por Categoria
            </h2>
            <p className="text-gray-400 mt-4 leading-relaxed">
              Visualize o total de gastos e receitas agrupados por tipo de
              despesa.
            </p>
          </div>
          <span className="text-blue-600 font-bold mt-6 flex items-center gap-2">
            Acessar Relatório{" "}
            <span className="group-hover:translate-x-2 transition-transform">
              →
            </span>
          </span>
        </button>
      </div>
    </div>
  );
}
