'use client'
import '../app/globals.css';
import React, { useState } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core'
import { faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons'
library.add(faEye, faEyeSlash)

const LoginPage = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [visible, setVisible] = useState(false);

  const handleLogin = (e) => {
    e.preventDefault();

    // implant "jwt" cookie
    document.cookie = "jwt=123456789; expires=Thu, 18 Dec 2021 12:00:00 UTC; path=/; SameSite=Strict; Secure";
    location.href = '/dashboard';
  };

  return (
    <>
      <div className='bg-gray-950 flex items-center justify-center h-[30em] py-72'>
        <form className='bg-slate-900 shadow-md rounded-[18px] px-10 pt-8 pb-8' onSubmit={handleLogin}>
          <h2 className='text-5xl font-bold mb-3 text-white text-center'>FantaClasse</h2>
          <h2 className='text-lg font-italic mb-10 text-gray-400 text-center'>Il primo FantaClasse digitale in Italia! <br /> Perché? La noia, brutta bestia.</h2>
          <div className='mb-5'>
            <input
              type='text'
              id='username'
              placeholder='Username'
              required={true}
              className='shadow appearance-none border border-gray-500 focus:border-white rounded-xl w-full py-3 px-4 bg-slate-900 text-white-700 leading-tight focus:outline-none focus:shadow-outline text-xl'
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
          </div>
          <div className='mb-10 relative'>
            <div className='relative'>
              <input
                type={visible ? 'text' : 'password'}
                id='password'
                placeholder='Password'
                required={true}
                className='shadow appearance-none border border-gray-500 focus:border-white rounded-xl w-full py-3 px-4 bg-slate-900 text-white-700 leading-tight focus:outline-none focus:shadow-outline text-xl'
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
              <span
                className='absolute inset-y-0 right-0 flex items-center pr-3'
                onClick={() => setVisible(!visible)}
              >
                <FontAwesomeIcon
                  icon={visible ? faEyeSlash : faEye}
                  className='text-gray-500 cursor-pointer text-lg'
                />
              </span>
            </div>
          </div>
          <div className='items-center'>
            <button
              type='submit'
              className='bg-blue-950 hover:bg-blue-500 text-white font-bold py-3 px-4 rounded-md focus:outline-none focus:shadow-outline w-full text-xl'
            >
              Accedi
            </button>
          </div>
        </form>
      </div>
      <div className='bg-gray-950 flex items-center justify-center'>
        <div className='bg-slate-900 shadow-md rounded-[18px] px-10 pt-8 pb-8 mb-8'>
        <h2 className='text-4xl font-bold text-white text-left'>FAQ</h2>
          <h2 className='text-lg font-italic mt-6 text-gray-400 text-left'> <b>- Non hai un account?</b><br />Chiedi a Roccuccio di creartene uno! <br /><b>- Non ricordi le tue credenziali?</b><br />Fottiti!<br /><b>- Ma non è che mi hackeri il telefono?</b><br />Suiciditi.</h2>
        </div>
      </div>
    </>
  );
};

export default LoginPage;
