import { useState } from 'react';

export default function FeedbackForm() {
  const [isSent, setIsSent] = useState(false);
  const [message, setMessage] = useState('');
  return (
    <>
      {isSent 
        ? 
          <h1>Thank you {message}!</h1> 
        :       
          <form onSubmit={e => {
            e.preventDefault();
            alert(`Sending: "${message}"`);
            setIsSent(true);
          }}>
            <textarea
              placeholder="Message"
              value={message}
              onChange={e => setMessage(e.target.value)}
            />
            <br />
            <button type="submit">Send</button>
          </form>
      }
    </>
  );  
}
