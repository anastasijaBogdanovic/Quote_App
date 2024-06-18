import React from 'react';
import Quote from './Quote';
import './QuoteList.css'

const QuoteList = ({ quotes, onRate }) => {
  return (
    <div className="quote-list">
      {quotes.map((quote) => (
        <Quote key={quote.id} quote={quote} onRate={onRate} />
      ))}
    </div>
  );
};


export default QuoteList;