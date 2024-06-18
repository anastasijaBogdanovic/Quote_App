import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faChevronUp, faChevronDown, faChevronLeft, faChevronRight } from '@fortawesome/free-solid-svg-icons';
import './Quote.css'
import PropTypes from 'prop-types';


const Quote = ({ quote, onRate }) => {
  const [vote, setVote] = React.useState(null);

  const handleRate = (value) => {
    onRate(quote.id, value);
    setVote(value);
  };

  const positiveVoteClass = vote === true ? 'positive-vote' : '';
  const negativeVoteClass = vote === false ? 'negative-vote' : '';
  
  let color;
  let percentage = quote.percentage;
  if (percentage >= 90 && percentage <= 100) {
    color = 'limegreen';
  } else if (percentage >= 80 && percentage < 90) {
    color = 'lime';
  } else if (percentage >= 70 && percentage < 80) {
    color = 'greenyellow';
  } else if (percentage >= 60 && percentage < 70) {
    color = 'yellow';
  } else if (percentage >= 50 && percentage < 60) {
    color = 'orange';
  } else if (percentage >= 40 && percentage < 50) {
    color = 'darkorange';
  } else if (percentage >= 30 && percentage < 40) {
    color = 'orangered';
  } else {
    color = 'red';
  }

  return (
    <div className="quote">
      <div className="quote-actions">
        <button className={`rate-button ${positiveVoteClass}`} onClick={() => handleRate(true)}>
          <FontAwesomeIcon icon={faChevronUp} className={`positive-icon ${positiveVoteClass}`} />
        </button>
        <span className="vote-count" style={{color: color}}>{quote.percentage}%</span>
        <span className="vote-count-rating">{quote.positiveRatingsCount}/{quote.negativeRatingsCount}</span>
        <button className={`rate-button ${negativeVoteClass}`} onClick={() => handleRate(false)}>
          <FontAwesomeIcon icon={faChevronDown} className={`negative-icon ${negativeVoteClass}`} />
        </button>
      </div>
      <div className="quote-content">
        <p className="quote-text">{quote.quote}</p>
        <p className="quote-author">- {quote.author}</p>
      </div>
    </div>
  );
};

Quote.propTypes = {
  quote: PropTypes.shape({
    id: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
    text: PropTypes.string,
    author: PropTypes.string,
    voteCount: PropTypes.number,
  }).isRequired,
  onRate: PropTypes.func.isRequired,
};


export default Quote;