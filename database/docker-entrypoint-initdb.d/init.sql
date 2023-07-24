--
-- PostgreSQL database dump
--

-- Dumped from database version 15.1
-- Dumped by pg_dump version 15.1

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

CREATE DATABASE picciottocanudobot; -- hardcoded shit for docker

\connect picciottocanudobot

--
-- Name: ads; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.ads (
    id integer NOT NULL,
    subject character varying,
    book character varying,
    book_code character varying,
    years integer[],
    contacts character varying[],
    from_user bigint
);


ALTER TABLE public.ads OWNER TO postgres;

--
-- Name: ads_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.ads_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.ads_id_seq OWNER TO postgres;

--
-- Name: ads_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ads_id_seq OWNED BY public.ads.id;


--
-- Name: subjects; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.subjects (
    code_name character varying NOT NULL,
    name character varying
);


ALTER TABLE public.subjects OWNER TO postgres;

--
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    user_id bigint NOT NULL,
    first_name character varying,
    last_name character varying,
    username character varying,
    lang character varying,
    status character varying,
    rank integer,
    banned boolean
);


ALTER TABLE public.users OWNER TO postgres;

--
-- Name: ads id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ads ALTER COLUMN id SET DEFAULT nextval('public.ads_id_seq'::regclass);


--
-- Name: ads ads_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ads
    ADD CONSTRAINT ads_pkey PRIMARY KEY (id);


--
-- Name: subjects subjects_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subjects
    ADD CONSTRAINT subjects_pkey PRIMARY KEY (code_name);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (user_id);


--
-- PostgreSQL database dump complete
--

