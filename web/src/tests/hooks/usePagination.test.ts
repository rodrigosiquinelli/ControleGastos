import { renderHook, act } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import { usePagination } from '../../hooks/usePagination';

describe('usePagination', () => {
  const mockItems = Array.from({ length: 25 }, (_, i) => i + 1);

  it('Deve_Iniciar_Com_Valores_Padrao', () => {
    const { result } = renderHook(() => usePagination(mockItems, 10));

    expect(result.current.currentPage).toBe(1);
    expect(result.current.totalPages).toBe(3);
    expect(result.current.currentItems).toHaveLength(10);
    expect(result.current.currentItems[0]).toBe(1);
  });

  it('Deve_Avancar_Para_Proxima_Pagina', () => {
    const { result } = renderHook(() => usePagination(mockItems, 10));

    act(() => {
      result.current.goToNext();
    });

    expect(result.current.currentPage).toBe(2);
    expect(result.current.currentItems[0]).toBe(11);
  });

  it('Nao_Deve_Ultrapassar_O_Limite_Maximo_De_Paginas', () => {
    const { result } = renderHook(() => usePagination(mockItems, 10));

    act(() => {
      result.current.goToNext();
      result.current.goToNext();
      result.current.goToNext();
    });

    expect(result.current.currentPage).toBe(3);
  });

  it('Deve_Voltar_Para_Pagina_Anterior', () => {
    const { result } = renderHook(() => usePagination(mockItems, 10));

    act(() => {
      result.current.goToNext();
      result.current.goToPrev();
    });

    expect(result.current.currentPage).toBe(1);
  });

  it('Nao_Deve_Ficar_Abaixo_Da_Pagina_Um', () => {
    const { result } = renderHook(() => usePagination(mockItems, 10));

    act(() => {
      result.current.goToPrev();
    });

    expect(result.current.currentPage).toBe(1);
  });

  it('Deve_Resetar_Para_Primeira_Pagina', () => {
    const { result } = renderHook(() => usePagination(mockItems, 10));

    act(() => {
      result.current.goToNext();
      result.current.goToNext();
      result.current.reset();
    });

    expect(result.current.currentPage).toBe(1);
  });
});
