import React, { useState, useEffect } from 'react';
import { Route, Routes } from 'react-router-dom';
import axios from 'axios';
import QuoteList from './components/QuoteList';
import Pagination from './components/Pagination';
import './App.css'

const App = () => {
  const [quotes, setQuotes] = useState([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);

  const fetchQuotes = async () => {
    const response = await axios.get(`http://localhost:5151/Quotes/GetQuotes?page=${currentPage}`);
    const { data } = response;
    setQuotes(data.quotes);
    setTotalPages(data.totalPages);
  };

  const handleRate = async (quoteId, value) => {
    console.log(quoteId);
    console.log(value);
    await axios.post(`http://localhost:5151/Quotes/RateQuote?quoteId=${quoteId}&value=${value}&userId=1`);
    fetchQuotes();
  };

  const handlePageChange = (page) => {
    setCurrentPage(page);
  };

  useEffect(() => {
    fetchQuotes();
  }, [currentPage]);

  return (
    <div className="App">
      <header className="App-header">
        <h1>Quotes</h1>
      </header>
      <QuoteList quotes={quotes} onRate={handleRate} />
      <Pagination
        currentPage={currentPage}
        totalPages={totalPages}
        onPageChange={handlePageChange}
      />
    </div>
  );
};

export default App;