import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faChevronLeft, faChevronRight } from '@fortawesome/free-solid-svg-icons';
import './Pagination.css'

const Pagination = ({ currentPage, totalPages, onPageChange }) => {
  const handlePageChange = (page) => {
    onPageChange(page);
  };

  return (
    <div className="pagination">
      {currentPage > 1 && (
        <a onClick={() => handlePageChange(currentPage - 1)}>
          <FontAwesomeIcon icon={faChevronLeft} />
        </a>
      )}
      <div className="page-info">
        <span>{currentPage} / {totalPages}</span>
      </div>
      {currentPage < totalPages && (
        <a onClick={() => handlePageChange(currentPage + 1)}>
          <FontAwesomeIcon icon={faChevronRight} />
        </a>
      )}
    </div>
  );
};

export default Pagination;