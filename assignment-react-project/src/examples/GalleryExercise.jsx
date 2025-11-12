import { useState } from 'react';
import { sculptureList } from './data.js';

const showStepNavigations = {
  next: true,
  previous: false
}

export default function Gallery() {
  const [index, setIndex] = useState(0);
  const [showMore, setShowMore] = useState(false);
  const [showPreviousOrNext, setShowPreviousOrNext] = useState(showStepNavigations);

  const isReachDestination = index + 1 == sculptureList.length;
  const isReachOrigin = index == 0;
  const {previous, next} = showPreviousOrNext;
  
  function handleNextClick() {
    if (index + 1 == sculptureList.length - 1)
    {
      setShowPreviousOrNext(currentState => ({...currentState, next : false}));
    }
    if (isReachDestination) return;        
    setIndex(index + 1);
    !previous && setShowPreviousOrNext(currentState => ({...currentState, previous : true}));
  }

  function handlePreviousClick() {
    if (index == 1) {
      setShowPreviousOrNext(currentState => ({...currentState, previous : false}));
    }
    if (isReachOrigin) return;
    setIndex(index - 1);
    !next && setShowPreviousOrNext(currentState => ({...currentState, next : true}));
  }

  function handleMoreClick() {
    setShowMore(!showMore);
  }

  let sculpture = sculptureList[index];
  return (
    <>
      <button onClick={handleNextClick} disabled={!next}>
        Next
      </button>  
      <button onClick={handlePreviousClick} disabled={!previous}>
        Previous
      </button>
      <h2>
        <i>{sculpture.name} </i> 
        by {sculpture.artist}
      </h2>
      <h3>  
        ({index + 1} of {sculptureList.length})
      </h3>
      <button onClick={handleMoreClick}>
        {showMore ? 'Hide' : 'Show'} details
      </button>
      {showMore && <p>{sculpture.description}</p>}
      <img 
        src={sculpture.url} 
        alt={sculpture.alt}
      />
    </>
  );
}
