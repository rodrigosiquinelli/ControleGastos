import { useState } from 'react';

// Hook customizado para gerenciar a lógica de paginação de listas de forma genérica e reutilizável.
export function usePagination<T>(items: T[], itemsPerPage: number = 10) {
  const [currentPage, setCurrentPage] = useState(1);

  // Realiza o cálculo do total de páginas e define o intervalo de itens visíveis na página atual.
  const totalPages = Math.ceil(items.length / itemsPerPage);
  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  const currentItems = items.slice(indexOfFirstItem, indexOfLastItem);

  const goToNext = () => setCurrentPage(prev => Math.min(prev + 1, totalPages));
  const goToPrev = () => setCurrentPage(prev => Math.max(prev - 1, 1));
  const reset = () => setCurrentPage(1);

  return {
    currentPage,
    totalPages,
    currentItems,
    goToNext,
    goToPrev,
    reset,
    itemsPerPage
  };
}
