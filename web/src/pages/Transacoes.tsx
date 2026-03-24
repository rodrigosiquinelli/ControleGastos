import { useEffect, useState } from 'react';
import api from '../services/api';
import { usePagination } from '../hooks/usePagination';

interface Pessoa { id: string; nome: string; idade: number; }
interface Categoria { id: string; descricao: string; finalidade: string; }
interface Transacao {
  id: string;
  descricao: string;
  valor: number;
  tipo: number;
  pessoaId: string;
  pessoaNome: string; 
  categoriaId: string;
}

// Componente para registro e listagem de transações.
export function Transacoes() {
  const [transacoes, setTransacoes] = useState<Transacao[]>([]);
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [descricao, setDescricao] = useState('');
  const [valor, setValor] = useState('');
  const [tipo, setTipo] = useState('Despesa');
  const [pessoaId, setPessoaId] = useState('');
  const [categoriaId, setCategoriaId] = useState('');

  const { currentItems, currentPage, totalPages, goToNext, goToPrev } = usePagination(transacoes, 10);

  useEffect(() => {
    carregarDadosIniciais();
  }, []);

  // Realiza chamadas paralelas à API para otimizar o tempo de carregamento inicial da tela.
  async function carregarDadosIniciais() {
    try {
      const [resT, resP, resC] = await Promise.all([
        api.get('/Transacoes'),
        api.get('/Pessoas'),
        api.get('/Categorias')
      ]);
      setTransacoes(resT.data || []);
      setPessoas(resP.data || []);
      setCategorias(resC.data || []);
    } catch (error) {
      console.error("Erro ao carregar dados:", error);
    }
  }

  // Valida as regras de negócio no front-end e envia o novo lançamento para o banco.
  async function salvar(e: React.FormEvent) {
    e.preventDefault();

    const valorNumerico = Number(valor);
    const tipoNumerico = tipo === 'Receita' ? 2 : 1;
    const p = pessoas.find(x => x.id === pessoaId);
    const c = categorias.find(x => x.id === categoriaId);

    // Validação: Impede o registro de transações com valores nulos ou negativos.
    if (valorNumerico <= 0) {
      return alert("⚠️ O valor da transação deve ser maior que zero.");
    }

    // Regra de Negócio: Bloqueia o registro de Receitas para usuários menores de 18 anos.
    if (p && p.idade < 18 && tipoNumerico === 2) {
      return alert("🚫 Menores de 18 anos não podem registrar Receitas!");
    }

    // Validação: Garante que o tipo da transação condiz com a finalidade da categoria.
    if (c) {
      const finalidade = c.finalidade; 
      if (finalidade === 'Receita' && tipoNumerico === 1) {
        return alert(`🚫 A categoria "${c.descricao}" é exclusiva para RECEITAS.`);
      }
      if (finalidade === 'Despesa' && tipoNumerico === 2) {
        return alert(`🚫 A categoria "${c.descricao}" é exclusiva para DESPESAS.`);
      }
    }

    try {
      await api.post('/Transacoes', { 
        descricao, 
        valor: valorNumerico, 
        tipo: tipoNumerico, 
        pessoaId, 
        categoriaId 
      });
      
      setDescricao(''); 
      setValor('');
      carregarDadosIniciais(); 
      alert("✅ Lançamento realizado com sucesso!");

    } catch (error: any) {
      const apiResponse = error.response?.data;
      const mensagemErro = 
        apiResponse?.Detailed || 
        apiResponse?.Message || 
        (typeof apiResponse === 'string' ? apiResponse : "Erro ao processar transação.");

      alert(`❌ Erro: ${mensagemErro}`);
    }
  }

  return (
    <div className="max-w-6xl mx-auto space-y-8 animate-in fade-in duration-500 text-left">
      <div className="border-l-4 border-blue-600 pl-4">
        <h1 className="text-3xl font-black text-gray-800">Lançamentos</h1>
        <p className="text-gray-500">Gestão de entradas e saídas financeiras</p>
      </div>

      {/* Formulário para novas transações financeiras */}
      <form onSubmit={salvar} className="bg-white p-8 rounded-3xl shadow-xl border border-gray-100 space-y-6">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="flex flex-col gap-2">
            <label className="text-sm font-bold text-gray-700">Descrição</label>
            <input 
              className="border-2 border-gray-100 rounded-2xl p-4 focus:border-blue-500 outline-none bg-gray-50/50"
              value={descricao} onChange={e => setDescricao(e.target.value)} required 
            />
          </div>
          <div className="flex flex-col gap-2">
            <label className="text-sm font-bold text-gray-700">Valor</label>
            <input 
              type="number" className="border-2 border-gray-100 rounded-2xl p-4 focus:border-blue-500 outline-none bg-gray-50/50"
              value={valor} onChange={e => setValor(e.target.value)} step="0.01" required 
            />
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <select className="border-2 border-gray-100 rounded-2xl p-4 bg-gray-50/50" value={tipo} onChange={e => setTipo(e.target.value)}>
            <option value="Despesa">🔴 Despesa</option>
            <option value="Receita">🟢 Receita</option>
          </select>

          <select className="border-2 border-gray-100 rounded-2xl p-4 bg-gray-50/50" value={pessoaId} onChange={e => setPessoaId(e.target.value)} required>
            <option value="">Responsável...</option>
            {pessoas.map(p => <option key={p.id} value={p.id}>{p.nome}</option>)}
          </select>

          <select className="border-2 border-gray-100 rounded-2xl p-4 bg-gray-50/50" value={categoriaId} onChange={e => setCategoriaId(e.target.value)} required>
            <option value="">Categoria...</option>
            {categorias.map(c => <option key={c.id} value={c.id}>{c.descricao}</option>)}
          </select>
        </div>

        <button className="w-full bg-blue-600 hover:bg-blue-700 text-white font-black py-5 rounded-2xl shadow-lg transition-all active:scale-[0.98]">
          FINALIZAR LANÇAMENTO
        </button>
      </form>

      {/* Tabela de exibição histórica dos lançamentos*/}
      <div className="bg-white rounded-3xl shadow-sm border border-gray-100 overflow-hidden">
        <table className="w-full text-left">
          <thead className="bg-gray-50/50 border-b border-gray-100">
            <tr>
              <th className="px-8 py-5 text-xs font-bold text-gray-400 uppercase tracking-widest">Lançamento / Responsável</th>
              <th className="px-8 py-5 text-xs font-bold text-gray-400 uppercase tracking-widest text-right">Valor</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-50">
            {currentItems.map(t => (
              <tr key={t.id} className="hover:bg-blue-50/20 transition-colors">
                <td className="px-8 py-6">
                  <div className="flex flex-col">
                    <span className="font-bold text-gray-800">{t.descricao}</span>
                    <div className="flex items-center gap-2 mt-1">
                       <span className={`text-[10px] font-black uppercase px-2 py-0.5 rounded ${t.tipo === 2 ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'}`}>
                         {t.tipo === 2 ? 'Receita' : 'Despesa'}
                       </span>
                       <span className="text-[10px] font-bold text-gray-400 uppercase tracking-tight">
                         👤 {t.pessoaNome || 'N/A'}
                       </span>
                    </div>
                  </div>
                </td>
                <td className={`px-8 py-6 text-right font-black text-lg ${t.tipo === 2 ? 'text-green-600' : 'text-red-600'}`}>
                  {t.tipo === 2 ? '+' : '-'} R$ {t.valor.toFixed(2)}
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        {/* Controles de Navegação da Paginação */}
        {totalPages > 1 && (
          <div className="p-6 bg-gray-50/30 border-t flex items-center justify-between">
            <span className="text-sm text-gray-500 font-black uppercase">
              Página {currentPage} de {totalPages}
            </span>
            <div className="flex gap-2">
              <button onClick={goToPrev} disabled={currentPage === 1} className="px-6 py-2 rounded-xl bg-white border-2 border-gray-100 font-black text-gray-600 hover:bg-gray-50 disabled:opacity-30 transition-all shadow-sm">Anterior</button>
              <button onClick={goToNext} disabled={currentPage === totalPages} className="px-6 py-2 rounded-xl bg-white border-2 border-gray-100 font-black text-gray-600 hover:bg-gray-50 disabled:opacity-30 transition-all shadow-sm">Próxima</button>
            </div>
          </div>
        )}
        
        {transacoes.length === 0 && <p className="p-20 text-center text-gray-400 italic font-medium">Nenhum lançamento registrado.</p>}
      </div>
    </div>
  );
}
