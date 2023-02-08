--
-- PostgreSQL database cluster dump
--

SET default_transaction_read_only = off;

SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;

\connect webapi

--
-- PostgreSQL database dump
--

-- Dumped from database version 10.4 (Debian 10.4-2.pgdg90+1)
-- Dumped by pg_dump version 10.4 (Debian 10.4-2.pgdg90+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner:
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner:
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: place_names; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.place_names (
    id integer NOT NULL,
    place character varying(250) NOT NULL,
    address_system character varying(250) NOT NULL,
    weight integer
);


ALTER TABLE public.place_names OWNER TO postgres;

--
-- Name: Placenames_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Placenames_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."Placenames_id_seq" OWNER TO postgres;

--
-- Name: Placenames_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Placenames_id_seq" OWNED BY public.place_names.id;


--
-- Name: accounts; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.accounts (
    id integer NOT NULL,
    email character varying(512),
    first_name character varying(128),
    last_name character varying(128),
    admin boolean DEFAULT false,
    keys_used integer DEFAULT 0,
    keys_allowed integer DEFAULT 5,
    company character varying(512),
    job_category character varying(256),
    job_title character varying(256),
    experience integer,
    contact_route character varying(128),
    email_confirmed boolean DEFAULT false,
    email_confirmation_date date,
    email_confirmation_tries smallint DEFAULT 1,
    email_key character varying(512),
    salt character varying(128),
    password character varying
);


ALTER TABLE public.accounts OWNER TO postgres;

--
-- Name: accounts_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.accounts_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.accounts_id_seq OWNER TO postgres;

--
-- Name: accounts_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.accounts_id_seq OWNED BY public.accounts.id;


--
-- Name: apikeys; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.apikeys (
    id integer NOT NULL,
    account_id integer,
    key character varying(20),
    elevated boolean DEFAULT false,
    enabled boolean,
    deleted boolean,
    configuration character varying(20),
    created_at_ticks bigint,
    pattern character varying(500),
    regex_pattern character varying(500),
    is_machine_name boolean,
    notes character varying,
    type character varying(10)
);


ALTER TABLE public.apikeys OWNER TO postgres;

--
-- Name: apikeys_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.apikeys_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.apikeys_id_seq OWNER TO postgres;

--
-- Name: apikeys_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.apikeys_id_seq OWNED BY public.apikeys.id;


--
-- Name: delivery_points; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.delivery_points (
    id integer NOT NULL,
    address_system character varying(250) NOT NULL,
    place character varying(250) NOT NULL,
    zip integer NOT NULL,
    x numeric(11,4) NOT NULL,
    y numeric(11,4)
);


ALTER TABLE public.delivery_points OWNER TO postgres;

--
-- Name: delivery_points_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.delivery_points_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.delivery_points_id_seq OWNER TO postgres;

--
-- Name: delivery_points_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.delivery_points_id_seq OWNED BY public.delivery_points.id;


--
-- Name: po_boxes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.po_boxes (
    id integer NOT NULL,
    zip integer NOT NULL,
    x numeric(10,2) NOT NULL,
    y numeric(10,2) NOT NULL
);


ALTER TABLE public.po_boxes OWNER TO postgres;

--
-- Name: poboxes_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.poboxes_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.poboxes_id_seq OWNER TO postgres;

--
-- Name: poboxes_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.poboxes_id_seq OWNED BY public.po_boxes.id;


--
-- Name: zip_codes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.zip_codes (
    id integer NOT NULL,
    zip integer NOT NULL,
    address_system character varying(50) NOT NULL,
    weight integer
);


ALTER TABLE public.zip_codes OWNER TO postgres;

--
-- Name: zip_corrections; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.zip_corrections (
    id integer NOT NULL,
    place character varying(25) NOT NULL,
    zip integer NOT NULL,
    zip_9 integer NOT NULL
);


ALTER TABLE public.zip_corrections OWNER TO postgres;

--
-- Name: zip_corrections_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.zip_corrections_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.zip_corrections_id_seq OWNER TO postgres;

--
-- Name: zip_corrections_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.zip_corrections_id_seq OWNED BY public.zip_corrections.id;


--
-- Name: zipcodes_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.zipcodes_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.zipcodes_id_seq OWNER TO postgres;

--
-- Name: zipcodes_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.zipcodes_id_seq OWNED BY public.zip_codes.id;


--
-- Name: accounts id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accounts ALTER COLUMN id SET DEFAULT nextval('public.accounts_id_seq'::regclass);


--
-- Name: apikeys id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.apikeys ALTER COLUMN id SET DEFAULT nextval('public.apikeys_id_seq'::regclass);


--
-- Name: delivery_points id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.delivery_points ALTER COLUMN id SET DEFAULT nextval('public.delivery_points_id_seq'::regclass);


--
-- Name: place_names id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.place_names ALTER COLUMN id SET DEFAULT nextval('public."Placenames_id_seq"'::regclass);


--
-- Name: po_boxes id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.po_boxes ALTER COLUMN id SET DEFAULT nextval('public.poboxes_id_seq'::regclass);


--
-- Name: zip_codes id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.zip_codes ALTER COLUMN id SET DEFAULT nextval('public.zipcodes_id_seq'::regclass);


--
-- Name: zip_corrections id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.zip_corrections ALTER COLUMN id SET DEFAULT nextval('public.zip_corrections_id_seq'::regclass);


--
-- Data for Name: accounts; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.accounts (id, email, first_name, last_name, admin, keys_used, keys_allowed, company, job_category, job_title, experience, contact_route, email_confirmed, email_confirmation_date, email_confirmation_tries, email_key, salt, password) FROM stdin;
1	test@test.com	agrc	location matters	t	1	5	agrc	\N	\N	10	email	t	1980-01-01	1	\N	rq5vdoY5U2S5TxuXDbq8uQ==	5G8mnUzwpfiBXIMy4f64P1irPSh95NxlFBMvCwLkaqQ=
\.


--
-- Data for Name: apikeys; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.apikeys (id, account_id, key, elevated, enabled, deleted, configuration, created_at_ticks, pattern, regex_pattern, is_machine_name, notes, type) FROM stdin;
1	1	AGRC-Explorer	t	t	f	Development	636657370427710489	*	asdf	f	key for the api explorer	 Browser
\.


--
-- Data for Name: delivery_points; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.delivery_points (id, address_system, place, zip, x, y) FROM stdin;
1	ogden	internal revenue service	84401	418473.3709	4563753.5920
2	ogden	internal revenue service	84401	418473.3709	4563753.5920
3	ogden	defense depot ogden	84407	416309.2163	4568701.2970
4	ogden	weber state university	84408	420791.1564	4560556.0410
5	salt lake city	university medical center	84132	429203.5199	4513717.4640
6	salt lake city	utah state tax commission	84134	420124.5991	4514200.0740
7	salt lake city	beneficial life insurance	84145	424016.3799	4513681.8510
8	salt lake city	questar gas company	84111	425093.4479	4512700.9300
9	salt lake city	wells fargo bank	84111	424824.3774	4512897.6270
10	salt lake city	lds hospital	84143	425766.1970	4514555.0320
11	salt lake city	veterans administration hospital	84148	429045.5692	4512195.8530
12	salt lake city	church of jesus christ/lds	84150	424958.3290	4513717.8660
13	salt lake city	american express co	84184	419260.5080	4503230.8810
14	salt lake city	key bank	84101	423993.8224	4510841.2780
15	salt lake city	salt lake county complex	84114	425058.9030	4508803.5580
16	salt lake city	us postal service	84199	420557.0004	4508935.3480
17	salt lake city	utah state capitol	84114	425046.4843	4514424.9730
\.


--
-- Data for Name: place_names; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.place_names (id, place, address_system, weight) FROM stdin;
1	abraham	delta	0
2	acord lakes	acord lakes	0
3	adamsville	beaver	0
4	adamsville	adamsville	1
5	alpine	alpine	1
6	alpine	provo	0
7	alta	salt lake city	0
8	altamont	roosevelt	0
9	alton	alton	3
10	altonah	roosevelt	0
11	am fork	american fork	0
12	amalga	logan	0
13	amangiri	big water	0
14	american fk	american fork	2
15	american fk	provo	0
16	american fork	american fork	3
17	american fork	provo	1
18	aneth	blanding	0
19	angle	angle	0
20	annabella	annabella	0
21	antimony	antimony	0
22	apple valley	apple valley	0
23	aragonite	tooele	0
24	arcadia	roosevelt	0
25	arches	moab	0
26	aurora	aurora	0
27	austin	elsinore	0
28	avalon	roosevelt	0
29	avon	logan	0
30	axtell	axtell	1
31	axtell	manti	0
32	bacchus	salt lake city	0
33	ballard	roosevelt	0
34	bauer	tooele	0
35	bcc	salt lake city	1
36	bear river	brigham city	2
37	bear river city	brigham city	1
38	bear river cy	brigham city	0
39	beaver	beaver	0
40	beaver county	beaver	0
41	beaver dam	brigham city	0
42	beaver mountain	logan	1
43	beaver mtn	logan	0
44	benjamin	provo	0
45	bennet	roosevelt	0
46	bennett	roosevelt	0
47	benson	logan	0
48	beryl	beryl	0
49	beryl jct	beryl	0
50	beryl junction	beryl	1
51	bicknell	bicknell	0
52	big cottonwood canyon	salt lake city	3
53	big cottonwood canyon resorts	salt lake city	0
54	big cottonwood cyn	salt lake city	2
55	big water	big water	0
56	bingham canyon	salt lake city	1
57	bingham cyn	salt lake city	0
58	birch creek	birch creek	0
59	birdseye	provo	0
60	black rock	fillmore	0
61	blanding	blanding	0
62	bloomington	st george	0
63	bloomington hills	st george	0
64	blue creek	brigham city	0
65	bluebell	roosevelt	0
66	bluff	bluff	0
67	bluffdale	provo	0
68	bluffdale	salt lake city	1
69	bonanza	vernal	0
70	boneta	roosevelt	0
71	border	delta	0
72	bothwell	brigham city	0
73	bottle hollow	roosevelt	0
74	boulder	boulder	0
75	bountiful	bountiful	0
76	box elder county	brigham city	0
77	brian head	brian head	1
78	brianhead	brian head	0
79	bridgeland	roosevelt	0
80	brigham	brigham city	0
81	brigham city	brigham city	1
82	brighton	salt lake city	0
83	brookside	brookside	0
84	browns park	browns park	0
85	bryce	bryce canyon city	3
86	bryce	bryce canyon county	2
87	bryce canyon	bryce canyon city	2
88	bryce canyon city	bryce canyon city	1
89	bryce canyon county	bryce canyon county	5
90	bryce cyn city	bryce canyon city	0
91	bryce cyn cnty	bryce canyon county	3
92	bryce cyn county	bryce canyon county	4
93	bullfrog	bullfrog	0
94	burbank	delta	0
95	burmester	tooele	0
96	burrville	richfield	0
97	cache	logan	1
98	cache county	logan	0
99	cache jct	logan	1
100	cache junction	logan	0
101	caineville	caineville	0
102	callao	nephi	0
103	camp williams	provo	0
104	camp williams	salt lake city	1
105	cannonville	cannonville	0
106	canyon meadows	heber city	0
107	canyon rim	salt lake city	0
108	carbon county	price	0
109	carbonville	price	0
110	castle dale	castle dale	1
111	castle rock	henefer	0
112	castle valley	castle valley	0
113	castledale	castle dale	0
114	cedar city	cedar city	0
115	cedar	cedar city	0
116	cedar creek	brigham city	0
117	cedar fort	cedar fort	0
118	cedar hills	provo	1
119	cedar hls	provo	0
120	cedar springs	brigham city	0
121	cedar valley	cedar fort	1
122	cedarview	roosevelt	0
123	center creek	heber city	0
124	centerfield	centerfield	0
125	centerville	centerville	0
126	central	central	0
127	central valley	central valley	0
128	charleston	heber city	0
129	chester	manti	0
130	church wells	big water	0
131	circleville	circleville	0
132	cisco	moab	0
133	clarkston	clarkston	1
134	clarkston	logan	0
135	clawson	clawson	0
136	clay basin	clay basin	0
137	clear creek	price	0
138	clear lake	delta	0
139	clearfield	clearfield	0
140	cleveland	cleveland	0
141	clinton	clearfield	0
142	clive	tooele	0
143	clover	rush valley	0
144	coalville	coalville	0
145	college ward	logan	0
146	collinston	brigham city	0
147	colombia	east carbon	0
148	colton	provo	0
149	columbia	east carbon	0
150	copperton	salt lake city	0
151	corinne	brigham city	0
152	cornish	logan	0
153	cottonwd hts	salt lake city	0
154	cottonwood heights	salt lake city	4
155	cottonwood heights city	salt lake city	2
156	cottonwood hgts	salt lake city	3
157	cottonwood hts	salt lake city	1
158	cove	logan	0
159	cove fort	kanosh	0
160	covered bridge	spanish fork	0
161	crescent jct	green river	1
162	crescent junction	green river	0
163	croydon	morgan	0
164	dammeron	dammeron valley	0
165	dammeron valley	dammeron valley	2
166	dammeron vly	dammeron valley	1
167	daniel	heber city	0
168	davis county	clearfield	0
169	deep creek	roosevelt	0
170	deer canyon	heber city	0
171	deer canyon preserve	heber city	0
172	deer crest	heber city	0
173	deer lodge	deer lodge	0
174	deer mountain	heber city	0
175	deer mtn	heber city	0
176	deer valley	park city	0
177	delle	grantsville	0
178	delle	tooele	0
179	delta	delta	0
180	deseret	delta	0
181	desert mound	cedar city	0
182	devils slide	morgan	0
183	deweyville	brigham city	0
184	diamond mountain	diamond mountain	1
185	diamond mountain	vernal	1
186	diamond mtn	diamond mtn	0
187	diamond mtn	vernal	0
188	diamond valley	st george	0
189	dividend	provo	0
190	draper	salt lake city	0
191	dry fork	vernal	0
192	duchesne	duchesne	1
193	duchesne	roosevelt	0
194	duchesne county	roosevelt	0
195	duck creek	duck creek village	1
196	duck creek village	duck creek village	2
197	duck creek vlg	duck creek village	0
198	dugway	dugway	1
199	dugway	tooele	0
200	dutch john	dutch john	0
201	e carbon	east carbon	2
202	e carbon city	east carbon	3
203	e carbon sunnyside	east carbon	0
204	e layton	layton	0
205	e millcreek	salt lake city	0
206	eagle mountain	eagle mountain	3
207	eagle mountain	provo	1
208	eagle mtn	eagle mountain	2
209	eagle mtn	provo	0
210	east carbon	east carbon	2
211	east carbon city	east carbon	3
212	east carbon sunnyside	east carbon	0
213	east layton	layton	0
214	east millcreek	salt lake city	0
215	eastland	monticello	0
216	echo	henefer	0
217	eden	ogden	0
218	elberta	provo	0
219	elk meadows	elk meadows	1
220	elk meadows	beaver	0
221	elk meadow	elk meadows	1
222	elk meadow	beaver	0
223	elk ridge	elk ridge	1
224	elk ridge	provo	0
225	elmo	elmo	0
226	elsinore	elsinore	0
227	elwood	brigham city	0
228	emery	emery	0
229	emery county	green river	0
230	emigration canyon	salt lake city	2
231	emigration cyn	salt lake city	1
232	emigratn cyn	salt lake city	0
233	emory	henefer	0
234	enoch	cedar city	0
235	enterprise	enterprise	1
236	enterprise	morgan	0
237	ephraim	ephraim	1
238	ephraim	manti	0
239	erda	tooele	0
240	escalante	escalante	0
241	eskdale	delta	0
242	etna	brigham city	0
243	eureka	eureka	0
244	fairfield	fairfield	1
245	fairview	fairview	1
246	fairview	manti	0
247	farm creek	roosevelt	0
248	farmington	farmington	0
249	farr west	ogden	0
250	faust	tooele	0
251	fayette	fayette	1
252	fayette	nephi	0
253	ferron	ferron	0
254	fielding	brigham city	0
255	fielding	fielding	1
256	fillmore	fillmore	0
257	fish springs	snake valley	0
258	fishlake	fishlake	0
259	flaming gorge	greendale	0
260	flowell	fillmore	0
261	fort douglas	salt lake city	0
262	fort duchesne	roosevelt	0
263	fountain green	fountain green	3
264	fountain green	manti	1
265	fountain grn	fountain green	2
266	fountain grn	manti	0
267	francis	kamas	0
268	freedom	manti	0
269	fremont	fremont	0
270	fremont jct	fishlake	1
271	fremont junction	fishlake	0
272	fruit heights	kaysville	0
273	fruita	fruita	0
274	fruitland	roosevelt	0
275	fry canyon	blanding	0
276	ft douglas	salt lake city	0
277	gandy	delta	0
278	garden	garden city	0
279	garden city	garden city	0
280	garfield	bryce canyon county	0
281	garfield co	bryce canyon county	1
282	garfield county	bryce canyon county	2
283	garland	brigham city	0
284	garland	garland	1
285	garrison	delta	0
286	genola	genola	1
287	genola	provo	0
288	glendale	glendale	0
289	glenwilde	park city	0
290	glenwood	glenwood	0
291	gold hill	tooele	0
292	goshen	goshen	1
293	goshen	provo	0
294	goshute	snake valley	0
295	grand county	moab	0
296	granite	salt lake city	0
297	grantsville	grantsville	1
298	grantsville	tooele	0
299	green river	green river	0
300	greendale	greendale	0
301	greenville	greenville	1
302	greenville	beaver	0
303	greenwich	greenwich	0
304	greenwood	fillmore	0
305	grouse creek	brigham city	0
306	grover	grover	0
307	gunlock	gunlock	0
308	gunnison	gunnison	1
309	gunnison	nephi	0
310	gusher	roosevelt	0
311	hafb	hill air force base	0
312	hailstone	heber city	0
313	halchita	blanding	0
314	halls crossing	blanding	0
315	hamiltons fort	cedar city	0
316	hamlin valley	beryl	1
317	hamlin valley	minersville	0
318	hanksville	hanksville	0
319	hanna	roosevelt	0
320	harding	delta	0
321	harrisburg	hurricane	0
322	harrisville	ogden	0
323	hatch	hatch	0
324	hatton	fillmore	0
325	hayden	roosevelt	0
326	heber	heber city	0
327	heber city	heber city	1
328	helper	helper	1
329	helper	price	0
330	henefer	henefer	0
331	henrieville	henrieville	0
332	herriman	salt lake city	0
333	hiawatha	price	0
334	hideout	heber city	0
335	highland	provo	1
336	highland	salt lake city	0
337	hildale	hildale	0
338	hill air force base	hill air force base	1
339	hill afb	hill air force base	0
340	hinckley	hinckley	0
341	hite	monticello	0
342	hobble creek	provo	0
343	holden	holden	0
344	holladay	salt lake city	0
345	honeyville	brigham city	0
346	hooper	ogden	0
347	howell	brigham city	0
348	hoytsville	coalville	0
349	huntington	huntington	0
350	huntsville	ogden	0
351	hurricane	hurricane	0
352	hyde park	hyde park	1
353	hyde park	logan	0
354	hyrum	hyrum	1
355	hyrum	logan	0
356	ibapah	iibapah	0
357	independence	heber city	1
358	independence	roosevelt	0
359	independence town	heber city	0
360	indianola	manti	0
361	interlaken	midway	0
362	ioka	roosevelt	0
363	iosepa	tooele	0
364	iron county	cedar city	0
365	iron springs	cedar city	0
366	ivins	ivins	0
367	jensen	vernal	0
368	jeremy ranch	park city	0
369	jericho jct	nephi	0
370	jericho junction	nephi	1
371	jordanelle	heber city	0
372	joseph	joseph	0
373	juab county	nephi	0
374	junction	junction	0
375	kamas	kamas	0
376	kanab	kanab	0
377	kanarraville	kanarraville	0
378	kane county	kanab	0
379	kanesville	ogden	0
380	kanosh	fillmore	0
381	kanosh	kanosh	1
382	kaysville	kaysville	0
383	kearns	salt lake city	0
384	keetley	heber city	0
385	kelton	brigham city	0
386	kenilworth	helper	0
387	kenilworth	price	1
388	kimball jct	park city	0
389	kimball junction	park city	1
390	kingston	kingston	0
391	knolls	tooele	0
392	kolob	kolob	0
393	koosharem	koosharem	0
394	la sal	la sal	0
395	la sal jct	la sal	1
396	la sal junction	la sal	0
397	la verkin	la verkin	0
398	lake point	tooele	0
399	lake shore	provo	0
400	lakeside	brigham city	0
401	laketown	laketown	0
402	lakeview	provo	0
403	lapoint	roosevelt	0
404	lawrence	huntington	0
405	layton	layton	0
406	leamington	delta	0
407	leamington	leamington	1
408	leeds	leeds	0
409	leeton	roosevelt	0
410	lehi	lehi	1
411	lehi	provo	0
412	leland	provo	0
413	leota	roosevelt	0
414	levan	levan	0
415	lewiston	lewiston	1
416	lewiston	logan	0
417	liberty	ogden	0
418	lincoln	tooele	0
419	lindon	lindon	1
420	lindon	provo	0
421	little cottonwood canyon	salt lake city	2
422	little cottonwood cyn	salt lake city	1
423	littleton	morgan	0
424	llc	salt lake city	0
425	loa	loa	0
426	logan	logan	0
427	long valley jct	alton	2
428	long valley junc	alton	0
429	long valley junction	alton	1
430	low	tooele	0
431	lucin	brigham city	0
432	lund	cedar city	0
433	lyman	lyman	0
434	lynn	brigham city	0
435	lynndyl	lynndyl	0
436	maeser	vernal	0
437	magna	salt lake city	0
438	mammoth	eureka	0
439	mammoth creek	mammoth creek	1
440	mammoth crk	mammoth creek	0
441	manderfield	manderfield	1
442	manderfield	beaver	0
443	manila	manila	0
444	manti	manti	0
445	mantua	brigham city	0
446	mantua	mantua	1
447	mapleton	mapleton	1
448	mapleton	provo	0
449	marion	kamas	0
450	marriott	ogden	1
451	marriott-slaterville	ogden	6
452	marriott-slaterville city	ogden	5
453	martin	price	0
454	marysvale	marysvale	0
455	mayfield	manti	0
456	mayfield	mayfield	1
457	meadow	filllmore	0
458	meadow	meadow	1
459	meadowville	laketown	0
460	mendon	logan	0
461	mendon	mendon	1
462	mercur	tooele	0
463	mexican hat	blanding	0
464	middleton	st george	0
465	midvale	salt lake city	0
466	midway	midway	0
467	milburn	manti	0
468	milford	milford	0
469	millard county	fillmore	0
470	millcreek	salt lake city	0
471	miller creek	price	0
472	mills	nephi	0
473	mills jct	tooele	0
474	mills junction	tooele	1
475	millville	logan	0
476	millville	millville	1
477	milton	morgan	0
478	minersville	minersville	0
479	mirror lake	roosevelt	0
480	moab	moab	0
481	moark jct	spanish fork	0
482	modena	modena	0
483	mohrland	huntington	0
484	molen	ferron	0
485	mona	mona	0
486	monarch	roosevelt	0
487	monroe	monroe	0
488	monroe mountain	monroe mountain	1
489	monroe mtn	monroe mountain	0
490	montezuma creek	blanding	0
491	monticello	monticello	0
492	monument valley	blanding	0
493	moore	emery	0
494	morgan	morgan	0
495	morgan county	morgan	0
496	moroni	manti	0
497	moroni	moroni	1
498	mosby mountain	roosevelt	1
499	mosby mtn	roosevelt	0
500	motoqua	gunlock	0
501	mount carmel	orderville	0
502	mount carmel jct	orderville	2
503	mount carmel junction	orderville	0
504	mount olympus	salt lake city	1
505	mount pleasant	manti	1
506	mount pleasant	mount pleasant	3
507	mountain green	morgan	1
508	mountain home	roosevelt	1
509	mriott sltrvl	ogden	4
510	ms	ogden	0
511	ms city	ogden	3
512	msc	ogden	2
513	mt carmel	orderville	1
514	mt carmel jct	orderville	1
515	mt emmons	roosevelt	0
516	mt olympus	salt lake city	1
517	mt pleasant	manti	0
518	mt pleasant	mount pleasant	2
519	mtn green	morgan	0
520	mtn home	roosevelt	0
521	murray	salt lake city	0
522	myton	myton	1
523	myton	roosevelt	0
524	n creek	beaver	0
525	n logan	logan	0
526	n ogden	ogden	0
527	n salt lake	north salt lake	1
528	n snyderville basin	park city	0
529	north snyderville basin	park city	1
530	naples	vernal	0
531	natural bridges	blanding	0
532	natural bridges nm	blanding	0
533	navajo mountain	blanding	1
534	navajo mtn	blanding	0
535	neola	roosevelt	0
536	nephi	nephi	0
537	new harmony	new harmony	0
538	newcastle	newcastle	0
539	newton	logan	0
540	newton	newton	1
541	nibley	logan	0
542	north creek	beaver	0
543	north logan	logan	1
544	north ogden	ogden	0
545	north salt lake	north salt lake	1
546	north snyderville basin	park city	1
547	notom	notom	0
548	nsl	north salt lake	0
549	oak city	oak city	0
550	oak	oak city	0
551	oak creek	manti	0
552	oakley	kamas	0
553	oaks park res	vernal	0
554	oaks park reservoir	vernal	1
555	oasis	delta	0
556	ogden	ogden	0
557	old fishlake	old fishlake	0
558	old la sal	la sal	0
559	oljato	blanding	0
560	ophir	tooele	0
561	orangeville	orangeville	0
562	orderville	orderville	0
563	orem	orem	1
564	orem	provo	0
565	osiris	antimony	0
566	otter creek	otter creek	0
567	ouray	roosevelt	0
568	palmyra	provo	0
569	panguitch	panguitch	0
570	panguitch lake	panguitch lake	0
571	paradise	logan	0
572	paragonah	paragonah	0
573	park city	park city	1
574	park city	park city	0
575	park valley	brigham city	0
576	parowan	parowan	0
577	partoun	snake valley	0
578	payson	payson	1
579	payson	provo	0
580	pc	park city	0
581	penrose	brigham city	0
582	peoa	kamas	0
583	perry	brigham city	0
584	peter	logan	0
585	petersboro	logan	0
586	peterson	morgan	0
587	pickleville	garden city	0
588	pigeon hollow jct	manti	1
589	pigeon hollow junction	manti	0
590	pine canyon	tooele	0
591	pine valley	pine valley	0
592	pinon forest	roosevelt	0
593	pinto	pinto	0
594	pintura	toquerville	0
595	plain city	ogden	0
596	plain	ogden	0
597	pleasant grove	pleasant grove	3
598	pleasant grove	provo	1
599	pleasant grv	pleasant grove	2
600	pleasant grv	provo	0
601	pleasant view	ogden	0
602	plymouth	brigham city	0
603	ponderosa	ponderosa	1
604	ponderosa	beaver	0
605	portage	brigham city	0
606	porterville	morgan	0
607	price	price	0
608	promontory	brigham city	1
609	promontory point	brigham city	0
610	providence	logan	0
611	providence	providence	1
612	provo	provo	0
613	randlett	roosevelt	0
614	randolph	randolph	0
615	redmond	redmond	0
616	richfield	richfield	0
617	richmond	logan	0
618	richmond	richmond	1
619	richville	morgan	0
620	river heights	logan	0
621	riverdale	ogden	0
622	riverside	brigham city	0
623	riverton	salt lake city	0
624	rockville	rockville	1
625	rocky ridge	rocky ridge	1
626	rocky ridge	mona	0
627	roosevelt	roosevelt	0
628	rosette	brigham city	0
629	round valley	laketown	0
630	rowley	tooele	0
631	rowley jct	tooele	1
632	rowley junction	tooele	0
633	roy	ogden	0
634	rush valley	rush valley	1
635	rush valley	tooele	0
636	s jordan	salt lake city	1
637	s ogden	ogden	0
638	s salt lake	salt lake city	2
639	s snyderville basin	park city	0
640	s weber	ogden	0
641	s willard	brigham city	0
642	sage creek jct	randolph	1
643	sage creek junction	randolph	0
644	saint george	st george	0
645	salem	provo	0
646	salem	salem	1
647	salina	salina	0
648	salt lake	salt lake city	1
649	salt lake city	salt lake city	2
650	salt lake cnty	salt lake city	0
651	salt lake co	salt lake city	0
652	salt lake county	salt lake city	0
653	salt springs	tooele	0
654	samak	kamas	0
655	san juan county	monticello	0
656	sandy	salt lake city	0
657	sandy ranch	sandy ranch	0
658	sanpete county	manti	0
659	santa clara	santa clara	0
660	santaquin	provo	0
661	santaquin	santaquin	1
662	saratoga	provo	0
663	saratoga	saratoga springs	1
664	saratoga spgs	provo	0
665	saratoga spgs	saratoga springs	1
666	saratoga springs	provo	0
667	saratoga springs	saratoga springs	1
668	scipio	fillmore	0
669	scipio	scipio	1
670	scofield	price	0
671	scofield	scofield	1
672	sevier	sevier	0
673	sgu	st george	0
674	shivwits	st george	0
675	sigurd	sigurd	0
676	silver city	eureka	0
677	silver creek jct	park city	1
678	silver creek junction	park city	0
679	silver reef	leeds	0
680	silver summit	park city	0
681	sj	salt lake city	0
682	skull valley	tooele	0
683	sl ci	salt lake city	0
684	slaterville	ogden	0
685	slc	salt lake city	0
686	slco	salt lake city	0
687	smithfield	logan	0
688	smithfield	smithfield	1
689	snake valley	snake valley	0
690	snow basin	ogden	0
691	snowbasin	ogden	0
692	snowbird	salt lake city	0
693	snowville	brigham city	0
694	snowville	snowville	1
695	snyderville	park city	0
696	so salt lake	salt lake city	1
697	soldier summit	soldier summit	0
698	solitude	salt lake city	0
699	south jordan	salt lake city	1
700	south ogden	ogden	0
701	south salt lake	salt lake city	2
702	south snyderville basin	park city	0
703	south weber	ogden	0
704	south willard	brigham city	0
705	spanish fork	provo	0
706	spanish fork	spanish fork	1
707	spanish valley	monticello	0
708	spirit lake	spirit lake	0
709	spring city	manti	0
710	spring city	spring city	1
711	spring	manti	0
712	spring	spring city	1
713	spring glen	price	0
714	spring lake	provo	0
715	springdale	springdale	0
716	springville	provo	0
717	springville	springville	1
718	spry	panguitch	0
719	ssl	salt lake city	0
720	st george	st george	1
721	st. george	st george	0
722	standardville	price	0
723	standrod	brigham city	0
724	stansbury park	tooele	0
725	sterling	manti	0
726	sterling	sterling	1
727	stg	st george	0
728	stockton	stockton	0
729	stockton	tooele	0
730	stoddard	morgan	0
731	strawberry	strawberry	0
732	sugarville	delta	0
733	sulphurdale	sulphurdale	1
734	sulphurdale	beaver	0
735	sulpherdale	sulphurdale	1
736	sulpherdale	beaver	0
737	summit	summit	0
738	summit county	kamas	0
739	summit park	park city	0
740	summit point	la sal	0
741	sundance	provo	0
742	sunnyside	east carbon	1
743	sunnyside	price	0
744	sunset	clearfield	0
745	sutherland	delta	0
746	syracuse	clearfield	0
747	tabiona	roosevelt	0
748	talmage	roosevelt	0
749	taylor	ogden	0
750	taylor flat	browns park	0
751	taylors flat	browns park	1
752	taylorsville	salt lake city	0
753	teasdale	teasdale	0
754	terra	tooele	0
755	thatcher	brigham city	0
756	thistle	provo	0
757	thompson	thompson springs	1
758	thompson springs	thompson springs	0
759	ticaboo	ticaboo	0
760	timber lakes	heber city	0
761	tintic	eureka	0
762	tooele	tooele	0
763	tooele army depot	tooele	0
764	tooele county	tooele	0
765	topaz	delta	0
766	toquerville	toquerville	0
767	torrey	torrey	0
768	tremonton	brigham city	0
769	tremonton	tremonton	1
770	trenton	logan	0
771	trenton	trenton	1
772	tridell	roosevelt	0
773	tropic	tropic	0
774	trout creek	nephi	0
775	trout creek	snake valley	1
776	tselakai dezza	blanding	0
777	tucker	provo	0
778	tuhaye	heber city	0
779	ucolo	monticello	0
780	uintah	ogden	0
781	uintah county	vernal	0
782	university of utah	salt lake city	0
783	uofu	salt lake city	0
784	u of u	salt lake city	0
785	upalco	roosevelt	0
786	upton	coalville	0
787	utah county	provo	0
788	venice	venice	0
789	vernal	vernal	0
790	vernon	tooele	0
791	vernon	vernon	1
792	veyo	veyo	0
793	victory ranch	heber city	0
794	vineyard	provo	0
795	vineyard	vineyard	1
796	virgin	virgin	0
797	w bountiful	bountiful	0
798	w haven	ogden	0
799	w jordan	salt lake city	1
800	w mountain	provo	0
801	w point	clearfield	0
802	w valley	salt lake city	0
803	w valley city	salt lake city	1
804	w waren	ogden	0
805	w warren	ogden	0
806	w wood	price	0
807	wahsatch	henefer	0
808	wahweap	big water	0
809	wales	manti	0
810	wales	wales	1
811	wallsburg	wallsburg	0
812	wanship	wanship	0
813	warren	ogden	0
814	wasatch county	heber city	0
815	washakie	brigham city	0
816	washington	washington	1
817	washington city	washington	0
818	washington terrace	ogden	0
819	wattis	price	0
820	weber county	ogden	0
821	wellington	price	0
822	wellington	wellington	1
823	wellsville	logan	0
824	wellsville	wellsville	1
825	wendover	tooele	0
826	wendover	wendover	1
827	west bountiful	bountiful	0
828	west haven	ogden	0
829	west jordan	salt lake city	1
830	west mountain	provo	0
831	west point	clearfield	0
832	west valley	salt lake city	0
833	west valley city	salt lake city	1
834	west waren	ogden	0
835	west warren	ogden	0
836	west weber	ogden	0
837	west wood	price	0
838	westwater	moab	0
839	westwood	price	0
840	wheelon	brigham city	0
841	white city	salt lake city	0
842	white	salt lake city	0
843	white mesa	blanding	0
844	whiterocks	roosevelt	0
845	widtsoe	widtsoe	0
846	widtsoe jct	widtsoe	1
847	widtsoe junction	widtsoe	0
848	willard	brigham city	0
849	willard	willard	1
850	winchester hills	st george	0
851	wj	salt lake city	0
852	wolf creek	ogden	0
853	wolf creek ranch	heber city	0
854	woodland	kamas	0
855	woodland	woodland	1
856	woodland hills	provo	1
857	woodland hills	woodland hills	1
858	woodland hls	provo	0
859	woodland hls	woodland hills	0
860	woodrow	delta	0
861	woodruff	woodruff	0
862	woods cross	bountiful	0
863	wood cross	bountiful	0
864	woodside	green river	0
865	wvc	salt lake city	0
866	yost	brigham city	0
867	young ward	logan	0
868	zane	beryl	0
869	zion national park	kolob	2
870	zion national park	new harmony	1
871	zion national park	rockville	3
872	zion national park	springdale	5
873	zion national park	virgin	4
874	zion ntl park	kolob	2
875	zion ntl park	new harmony	1
876	zion ntl park	rockville	3
877	zion ntl park	springdale	5
878	zion ntl park	virgin	4
\.


--
-- Data for Name: po_boxes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.po_boxes (id, zip, x, y) FROM stdin;
1	84041	420091.92	4547682.01
2	84148	429045.57	4512195.85
3	84025	425600.88	4537015.58
4	84520	549767.01	4377249.11
5	84327	417979.11	4634996.12
6	84791	272577.68	4107691.65
7	84044	407620.16	4507336.64
8	84752	331518.41	4231575.96
9	84021	551345.90	4446104.14
10	84316	380315.28	4628085.71
11	84412	418700.76	4568473.89
12	84723	388260.52	4225606.61
13	84319	428868.23	4609535.58
14	84323	430403.19	4620758.71
15	84635	355900.39	4354031.67
16	84050	442983.46	4543348.50
17	84052	579862.65	4449752.78
18	84098	454331.22	4508184.66
19	84329	299322.69	4632469.89
20	84663	448118.02	4446504.48
21	84534	650093.99	4125117.04
22	84501	516234.04	4383198.41
23	84029	376078.56	4495414.23
24	84740	393178.47	4233654.99
25	84125	420572.88	4508922.30
26	84320	429047.03	4647301.10
27	84531	601236.44	4112030.81
28	84771	270537.51	4110326.44
29	84130	420572.88	4508922.30
30	84317	436178.68	4567335.73
31	84662	457355.72	4370010.49
32	84744	423135.53	4262799.08
33	84655	432911.42	4425390.04
34	84138	420572.88	4508922.30
35	84047	424547.75	4495869.66
36	84751	324305.94	4251357.83
37	84150	424958.33	4513717.87
38	84328	430314.42	4602490.42
39	84602	444952.53	4455549.91
40	84542	523559.86	4376982.86
41	84013	406311.13	4464440.98
42	84118	416616.82	4500501.42
43	84143	425766.20	4514555.03
44	84145	424187.54	4513160.76
45	84315	405702.74	4557622.18
46	84523	488426.75	4326872.59
47	84054	423510.50	4522302.43
48	84079	623486.75	4479399.78
49	84091	426127.82	4493648.93
50	84735	373512.09	4168273.03
51	84660	444472.27	4441498.93
52	84651	438848.31	4433009.43
53	84134	420124.60	4514200.07
54	84536	569596.23	4095728.64
55	84001	560678.67	4467852.70
56	84665	440165.71	4338672.64
57	84720	318118.34	4172625.86
58	84007	566389.27	4467922.49
59	84736	412209.07	4157762.94
60	84318	431466.10	4627765.09
61	84627	449570.93	4357118.98
62	84037	420892.02	4543553.99
63	84325	418571.27	4618129.01
64	84033	458176.95	4540668.23
65	84032	465160.10	4484363.27
66	84654	425609.30	4312475.21
67	84780	276995.64	4112244.43
68	84603	443843.57	4453699.19
69	84066	586119.38	4461371.09
70	84326	431311.70	4615110.11
71	84080	377826.67	4439025.50
72	84086	486457.62	4596845.72
73	84657	415999.59	4299366.74
74	84521	515724.61	4359997.64
75	84083	243623.92	4514012.91
76	84652	425019.79	4317771.91
77	84634	429059.00	4334102.56
78	84774	297451.85	4125090.52
79	84095	420455.34	4491219.52
80	84311	407237.84	4629762.04
81	84060	458077.08	4499366.77
82	84636	390149.44	4328665.77
83	84516	491606.26	4331432.07
84	84539	552705.90	4378164.23
85	84038	473036.91	4630408.44
86	84790	272577.68	4107691.65
87	84647	460669.50	4377652.74
88	84757	295920.82	4150589.35
89	84127	420572.88	4508922.30
90	84071	384642.91	4479248.85
91	84039	602151.43	4473304.48
92	84637	375118.03	4295590.82
93	84010	425762.32	4526800.23
94	84631	385465.88	4314125.28
95	84761	338945.09	4189798.13
96	84773	458281.72	4237424.02
97	84330	405183.45	4636815.97
98	84049	459758.52	4484674.95
99	84340	413333.46	4584310.19
100	84133	420572.88	4508922.30
101	84067	413856.60	4558718.72
102	84605	444231.67	4452556.93
103	84158	430255.04	4511488.27
104	84601	443843.57	4453699.19
105	84737	295093.00	4117038.07
106	84711	407818.76	4284962.97
107	84755	351800.79	4123049.01
108	84629	462206.29	4386599.38
109	84710	368877.05	4144720.23
110	84132	429203.52	4513717.46
111	84097	442669.77	4458580.23
112	84046	607496.31	4538629.89
113	84301	406187.52	4607846.73
114	84136	424016.38	4513681.85
115	84715	452544.22	4243725.89
116	84062	436911.81	4468185.28
117	84069	376477.72	4468199.38
118	84701	405734.76	4291729.40
119	84633	423065.26	4422722.70
120	84014	425929.60	4530237.18
121	84646	450521.54	4375306.23
122	84653	442796.66	4434309.48
123	84409	417414.15	4561054.17
124	84020	425249.70	4486882.03
125	84644	377689.13	4305327.55
126	84530	652985.00	4241952.08
127	84307	407320.27	4600429.67
128	84332	431119.65	4617971.13
129	84043	428403.85	4471207.01
130	84724	400067.50	4282274.53
131	84189	423993.82	4510841.28
132	84201	418473.37	4563753.59
133	84090	429522.19	4492809.12
134	84026	596617.82	4461936.49
135	84750	392539.41	4256625.75
136	84072	524632.83	4466878.83
137	84712	412318.82	4219272.06
138	84117	430431.68	4501977.42
139	84341	431470.06	4623810.05
140	84522	478472.75	4308250.04
141	84070	424103.00	4491349.93
142	84747	443897.98	4250654.36
143	84055	474664.24	4507122.37
144	84649	384956.50	4359405.48
145	84110	424187.54	4513160.76
146	84638	388997.08	4376816.07
147	84078	623486.75	4479399.78
148	84337	403025.96	4618265.60
149	84759	373431.50	4187221.00
150	84726	446724.43	4180544.55
151	84730	414152.53	4290940.89
152	84322	432374.00	4621563.95
153	84074	389977.35	4487571.02
154	84053	582293.35	4476414.54
155	84334	404454.34	4627073.00
156	84639	425726.71	4379086.52
157	84734	525170.71	4247361.31
158	84190	425058.90	4508803.56
159	84763	318730.03	4114726.37
160	84767	323010.17	4117952.91
161	84312	403303.63	4621714.82
162	84772	329588.52	4185486.92
163	84180	420572.88	4508922.30
164	84746	291261.57	4124452.24
165	84729	358474.82	4131282.29
166	84088	418223.12	4495635.16
167	84305	413240.02	4641687.27
168	84035	641193.97	4470168.08
169	84525	572621.09	4316575.68
170	84085	590665.93	4480088.14
171	84116	420679.47	4515545.89
172	84017	466507.27	4529622.30
173	84758	354693.59	4126782.06
174	84028	467226.58	4643954.54
175	84131	420572.88	4508922.30
176	84082	463925.72	4470911.02
177	84749	448559.07	4249831.07
178	84643	438763.91	4329789.97
179	84151	424187.54	4513160.76
180	84006	407311.79	4491151.59
181	84401	418686.03	4563121.43
182	84760	343780.24	4194716.84
183	84310	430680.25	4572999.16
184	84721	318118.34	4172625.86
185	84016	414444.98	4549188.13
186	84626	418288.45	4422927.99
187	84784	324392.04	4095312.01
188	84126	420572.88	4508922.30
189	84725	259632.65	4162022.27
190	84338	421808.71	4641085.74
191	84065	419816.60	4485923.17
192	84121	430152.53	4497623.47
193	84765	264234.75	4112837.77
194	84199	420572.88	4508922.30
195	84068	456695.13	4501590.24
196	84624	364852.90	4356912.31
197	84244	418473.37	4563753.59
198	84109	431595.39	4505644.43
199	84716	462787.20	4195748.36
200	84059	440368.16	4461040.29
201	84003	432142.01	4470022.95
202	84184	419260.51	4503230.88
203	84620	418761.21	4308430.96
204	84728	237101.27	4313768.81
205	84526	512499.67	4392978.87
206	84640	381975.04	4375287.95
207	84122	417426.52	4514394.84
208	84165	424538.23	4506100.19
209	84139	425093.45	4512700.93
210	84415	420228.60	4559006.60
211	84718	406822.54	4158203.86
212	84512	628187.35	4127289.40
213	84537	495385.57	4341510.62
214	84518	512282.72	4355508.57
215	84147	424187.54	4513160.76
216	84107	424923.80	4501725.80
217	84511	634329.81	4165479.53
218	84036	476159.43	4499305.39
219	84745	298320.47	4119752.32
220	84314	410043.29	4609951.42
221	84114	425046.48	4514424.97
222	84642	445083.84	4346645.24
223	84056	417687.12	4551447.39
224	84776	404495.74	4164704.16
225	84741	364114.78	4101225.30
226	84141	424824.38	4512897.63
227	84120	415776.19	4505612.07
228	84632	445529.56	4386610.78
229	84092	429522.19	4492809.12
230	84333	432976.91	4641479.07
231	84339	422467.45	4610118.72
232	84106	427478.39	4508883.26
233	84535	645775.04	4192693.66
234	84628	405727.40	4423715.81
235	84713	356404.72	4237476.67
236	84022	351061.93	4453722.34
237	84336	357977.74	4647337.98
238	84089	413912.67	4552038.28
239	84153	420572.88	4508922.30
240	84764	397942.53	4170209.90
241	84302	415070.86	4595871.72
242	84756	275153.84	4172340.36
243	84719	337001.35	4174824.78
244	84754	402358.38	4277474.77
245	84408	420791.16	4560556.04
246	84407	416309.22	4568701.30
247	84023	634762.90	4532406.57
248	84064	484584.24	4612493.63
249	84528	503137.05	4353243.00
250	84532	626315.58	4270549.86
251	84645	426757.47	4407901.49
252	84648	428355.80	4395773.06
253	84738	262544.45	4115731.08
254	84775	463082.25	4239065.02
255	84779	305771.30	4119725.66
256	84782	261449.93	4135740.18
257	84313	259689.37	4621144.00
258	82930	502483.26	4568371.38
259	84335	430926.25	4632348.62
260	84024	462680.34	4536443.85
261	84075	410636.46	4548802.06
262	84076	597503.23	4478587.86
263	84129	416616.82	4500501.42
264	84604	444557.20	4458491.17
265	84533	524242.75	4152668.01
266	84011	425762.32	4526800.23
267	84513	498255.80	4340174.02
268	84015	413912.67	4552038.28
269	84606	444231.67	4452556.93
270	84321	430403.19	4620758.71
271	84057	440368.16	4461040.29
272	84403	420228.60	4559006.60
273	84402	417414.15	4561054.17
274	84404	418700.76	4568473.89
275	84770	270537.51	4110326.44
276	84093	429522.19	4492809.12
277	84084	418223.12	4495635.16
278	84171	430152.53	4497623.47
279	84108	430255.04	4511488.27
280	84157	424923.80	4501725.80
281	84115	424538.23	4506100.19
282	84152	427478.39	4508883.26
283	84170	415776.19	4505612.07
284	84762	352434.49	4154222.79
285	84101	424187.54	4513160.76
286	86021	324392.04	4095312.01
\.


--
-- Data for Name: zip_codes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.zip_codes (id, zip, address_system, weight) FROM stdin;
1	82930	kamas	0
2	83342	brigham city	0
3	84001	roosevelt	0
4	84002	roosevelt	0
5	84003	american fork	2
6	84003	highland	1
7	84003	provo	0
8	84004	alpine	1
9	84004	provo	0
10	84005	eagle mountain	0
11	84006	salt lake city	0
12	84007	roosevelt	0
13	84008	vernal	0
14	84009	salt lake city	0
15	84010	bountiful	0
16	84011	bountiful	0
17	84013	cedar fort	2
18	84013	eagle mountain	1
19	84013	provo	0
20	84014	centerville	0
21	84015	clearfield	0
22	84017	coalville	2
23	84017	park city	0
24	84017	wanship	1
25	84018	morgan	0
26	84020	salt lake city	0
27	84021	duchesne	1
28	84021	roosevelt	0
29	84022	dugway	0
30	84023	browns park	2
31	84023	clay basin	3
32	84023	deer lodge	4
33	84023	diamond mountain	1
34	84023	dutch john	6
35	84023	greendale	5
36	84023	spirit lake	0
37	84024	henefer	0
38	84025	farmington	0
39	84026	roosevelt	0
40	84027	roosevelt	0
41	84028	garden city	0
42	84029	grantsville	1
43	84029	tooele	0
44	84031	roosevelt	0
45	84032	heber city	2
46	84032	soldier summit	1
47	84032	strawberry	0
48	84033	henefer	0
49	84034	ibapah	0
50	84034	snake valley	1
51	84035	vernal	0
52	84036	heber city	0
53	84036	kamas	2
54	84036	woodland	1
55	84037	kaysville	1
56	84037	layton	0
57	84038	laketown	0
58	84039	roosevelt	0
59	84040	layton	0
60	84041	layton	0
61	84042	lindon	0
62	84043	eagle mountain	2
63	84043	lehi	3
64	84043	provo	0
65	84043	saratoga springs	1
66	84044	salt lake city	0
67	84045	provo	0
68	84045	saratoga springs	1
69	84046	birch creek	1
70	84046	manila	2
71	84046	spirit lake	0
72	84047	salt lake city	0
73	84049	interlaken	0
74	84049	midway	0
75	84050	morgan	0
76	84051	roosevelt	0
77	84052	myton	1
78	84052	roosevelt	0
79	84053	roosevelt	0
80	84054	north salt lake	0
81	84055	kamas	0
82	84056	clearfield	0
83	84056	hill air force base	1
84	84057	orem	1
85	84057	vineyard	0
86	84058	orem	1
87	84058	vineyard	0
88	84059	orem	1
89	84059	vineyard	0
90	84060	heber city	0
91	84060	park city	1
92	84061	kamas	0
93	84062	pleasant grove	1
94	84062	provo	0
95	84063	roosevelt	0
96	84064	randolph	0
97	84065	salt lake city	0
98	84066	roosevelt	0
99	84067	ogden	0
100	84068	park city	0
101	84069	rush valley	1
102	84069	tooele	0
103	84070	salt lake city	0
104	84071	stockton	1
105	84071	tooele	0
106	84072	roosevelt	0
107	84073	roosevelt	0
108	84074	tooele	0
109	84075	clearfield	0
110	84076	roosevelt	0
111	84078	vernal	0
112	84079	vernal	0
113	84080	tooele	0
114	84080	vernon	1
115	84081	salt lake city	0
116	84082	wallsburg	0
117	84083	snake valley	0
118	84083	wendover	1
119	84084	salt lake city	0
120	84085	roosevelt	0
121	84086	woodruff	0
122	84087	bountiful	0
123	84088	salt lake city	0
124	84089	clearfield	0
125	84090	sandy	0
126	84091	sandy	0
127	84092	salt lake city	0
128	84093	salt lake city	0
129	84094	salt lake city	0
130	84095	salt lake city	0
131	84096	salt lake city	0
132	84097	orem	0
133	84098	park city	0
134	84101	salt lake city	0
135	84102	salt lake city	0
136	84103	salt lake city	0
137	84104	salt lake city	0
138	84105	salt lake city	0
139	84106	salt lake city	0
140	84107	salt lake city	0
141	84108	salt lake city	0
142	84109	salt lake city	0
143	84110	salt lake city	0
144	84111	salt lake city	0
145	84112	salt lake city	0
146	84113	salt lake city	0
147	84114	salt lake city	0
148	84115	salt lake city	0
149	84116	salt lake city	0
150	84117	salt lake city	0
151	84118	salt lake city	0
152	84119	salt lake city	0
153	84120	salt lake city	0
154	84121	salt lake city	0
155	84122	salt lake city	0
156	84122	salt lake city	0
157	84123	salt lake city	0
158	84124	salt lake city	0
159	84125	salt lake city	0
160	84126	salt lake city	0
161	84128	salt lake city	0
162	84129	salt lake city	0
163	84130	salt lake city	0
164	84132	salt lake city	0
165	84134	salt lake city	0
166	84136	salt lake city	0
167	84138	salt lake city	0
168	84139	salt lake city	0
169	84141	salt lake city	0
170	84143	salt lake city	0
171	84145	salt lake city	0
172	84147	salt lake city	0
173	84148	salt lake city	0
174	84150	salt lake city	0
175	84150	salt lake city	0
176	84151	salt lake city	0
177	84152	salt lake city	0
178	84157	salt lake city	0
179	84158	salt lake city	0
180	84165	salt lake city	0
181	84170	salt lake city	0
182	84171	salt lake city	0
183	84184	salt lake city	0
184	84189	salt lake city	0
185	84190	salt lake city	0
186	84199	salt lake city	0
187	84201	ogden	0
188	84244	ogden	0
189	84301	brigham city	0
190	84302	brigham city	0
191	84304	logan	0
192	84305	clarkston	1
193	84305	logan	0
194	84306	brigham city	0
195	84306	logan	1
196	84307	brigham city	0
197	84308	lewiston	1
198	84308	logan	0
199	84309	brigham city	0
200	84310	ogden	0
201	84311	brigham city	0
202	84311	fielding	1
203	84312	brigham city	0
204	84312	garland	1
205	84313	brigham city	0
206	84314	brigham city	0
207	84315	ogden	0
208	84316	brigham city	0
209	84317	ogden	0
210	84318	hyde park	1
211	84318	logan	0
212	84319	hyrum	1
213	84319	logan	0
214	84320	lewiston	1
215	84320	logan	0
216	84321	logan	0
217	84322	logan	0
218	84323	logan	0
219	84324	brigham city	0
220	84324	mantua	1
221	84325	logan	0
222	84325	mendon	1
223	84326	millville	0
224	84327	logan	0
225	84327	newton	1
226	84328	logan	0
227	84329	brigham city	0
228	84330	brigham city	0
229	84331	brigham city	0
230	84332	providence	0
231	84333	logan	0
232	84333	richmond	1
233	84334	brigham city	0
234	84335	logan	0
235	84335	smithfield	1
236	84336	brigham city	0
237	84336	snowville	1
238	84337	brigham city	0
239	84337	tremonton	1
240	84338	trenton	0
241	84339	logan	0
242	84339	wellsville	1
243	84340	brigham city	0
244	84340	willard	1
245	84341	logan	0
246	84401	ogden	0
247	84402	ogden	0
248	84403	ogden	0
249	84404	ogden	0
250	84405	ogden	0
251	84407	ogden	0
252	84408	ogden	0
253	84409	ogden	0
254	84412	ogden	0
255	84414	ogden	0
256	84415	ogden	0
257	84501	elmo	1
258	84501	price	0
259	84501	price	2
260	84510	blanding	0
261	84511	blanding	0
262	84512	bluff	0
263	84513	castle dale	0
264	84515	moab	0
265	84516	clawson	0
266	84518	cleveland	0
267	84520	east carbon	1
268	84520	price	0
269	84521	cleveland	0
270	84521	elmo	1
271	84522	emery	0
272	84523	ferron	0
273	84525	green river	0
274	84526	helper	2
275	84526	price	0
276	84526	scofield	1
277	84528	huntington	0
278	84529	price	0
279	84530	la sal	0
280	84531	blanding	0
281	84532	castle valley	1
282	84532	moab	2
283	84532	monticello	0
284	84533	bullfrog	1
285	84533	sandy ranch	0
286	84533	ticaboo	2
287	84534	blanding	0
288	84535	monticello	0
289	84536	blanding	0
290	84537	orangeville	0
291	84539	price	0
292	84540	green river	0
293	84540	moab	0
294	84540	thompson springs	1
295	84542	price	0
296	84542	wellington	1
297	84601	provo	0
298	84602	provo	0
299	84603	provo	0
300	84604	heber city	0
301	84604	lindon	1
302	84604	provo	2
303	84605	provo	0
304	84606	provo	0
305	84620	aurora	0
306	84621	axtel	1
307	84621	manti	0
308	84622	centerfield	1
309	84622	manti	0
310	84623	manti	0
311	84624	delta	3
312	84624	fillmore	0
313	84624	hinckley	2
314	84624	lynndyl	1
315	84626	provo	0
316	84627	ephraim	1
317	84627	manti	0
318	84628	eureka	0
319	84629	fairview	2
320	84629	manti	1
321	84629	provo	0
322	84630	fayette	1
323	84630	manti	0
324	84631	fillmore	0
325	84632	fountain green	1
326	84632	manti	0
327	84633	goshen	1
328	84633	provo	0
329	84634	gunnison	1
330	84634	manti	0
331	84635	delta	0
332	84635	hinckley	1
333	84636	fillmore	0
334	84636	holden	1
335	84637	fillmore	0
336	84637	kanosh	1
337	84638	delta	0
338	84638	leamington	1
339	84639	levan	0
340	84640	delta	0
341	84640	lynndyl	1
342	84642	manti	0
343	84643	manti	0
344	84643	mayfield	1
345	84644	fillmore	0
346	84644	meadow	1
347	84645	mona	0
348	84645	rocky ridge	1
349	84646	manti	0
350	84646	moroni	1
351	84647	manti	0
352	84647	mount pleasant	1
353	84648	nephi	0
354	84649	delta	0
355	84649	oak city	1
356	84651	elk ridge	1
357	84651	payson	2
358	84651	provo	0
359	84652	redmond	0
360	84653	provo	0
361	84653	salem	2
362	84653	woodland hills	1
363	84654	acord lakes	0
364	84654	fishlake	1
365	84654	salina	2
366	84655	genola	1
367	84655	provo	0
368	84655	santaquin	2
369	84656	scipio	0
370	84657	sigurd	0
371	84660	provo	0
372	84660	spanish fork	1
373	84662	manti	0
374	84662	spring city	1
375	84663	provo	0
376	84663	springville	1
377	84664	mapleton	2
378	84664	provo	0
379	84664	springville	1
380	84665	manti	0
381	84665	sterling	1
382	84667	manti	0
383	84667	wales	1
384	84701	richfield	1
385	84701	venice	0
386	84710	alton	0
387	84711	annabella	0
388	84712	angle	1
389	84712	antimony	2
390	84712	otter creek	0
391	84713	beaver	4
392	84713	elk meadows	3
393	84713	ponderosa	2
394	84713	greenville	1
395	84713	sulphurdale	0
396	84714	beryl	0
397	84715	bicknell	0
398	84716	boulder	1
399	84716	sandy ranch	0
400	84718	cannonville	0
401	84719	brian head	0
402	84720	cedar city	0
403	84721	cedar city	0
404	84722	central	0
405	84723	circleville	0
406	84724	elsinore	0
407	84725	enterprise	0
408	84726	escalante	0
409	84728	delta	1
410	84728	fillmore	0
411	84729	glendale	0
412	84730	glenwood	0
413	84731	beaver	0
414	84731	greenville	2
415	84731	adamsville	1
416	84732	greenwich	0
417	84733	gunlock	0
418	84765	gunlock	1
419	84734	hanksville	0
420	84735	hatch	1
421	84735	mammoth creek	0
422	84736	henrieville	0
423	84737	apple valley	0
424	84737	hurricane	1
425	84738	ivins	1
426	84739	joseph	0
427	84740	junction	0
428	84741	big water	2
429	84741	bullfrog	0
430	84741	cannonville	1
431	84741	kanab	3
432	84742	kanarraville	0
433	84743	kingston	0
434	84744	koosharem	0
435	84745	hurricane	0
436	84745	la verkin	1
437	84746	leeds	0
438	84747	fremont	2
439	84747	loa	1
440	84747	old fishlake	0
441	84749	lyman	0
442	84750	marysvale	0
443	84751	milford	0
444	84752	minersville	0
445	84753	modena	0
446	84754	central valley	1
447	84754	monroe	2
448	84754	monroe mountain	0
449	84755	mount carmel	1
450	84755	orderville	0
451	84756	enterprise	0
452	84756	newcastle	2
453	84756	pinto	1
454	84757	new harmony	1
455	84757	toquerville	0
456	84758	glendale	0
457	84758	orderville	1
458	84759	panguitch	2
459	84759	panguitch lake	1
460	84759	widtsoe	0
461	84760	paragonah	0
462	84761	parowan	0
463	84762	duck creek village	0
464	84763	rockville	0
465	84764	bryce canyon city	1
466	84764	bryce canyon county	0
467	84765	santa clara	0
468	84766	sevier	0
469	84767	kolob	0
470	84767	springdale	1
471	84770	st george	0
472	84771	st george	0
473	84772	summit	0
474	84773	grover	0
475	84773	teasdale	1
476	84774	toquerville	0
477	84775	caineville	2
478	84775	fruita	1
479	84775	notom	0
480	84775	torrey	3
481	84776	tropic	0
482	84779	virgin	0
483	84780	st george	0
484	84780	washington	1
485	84781	pine valley	1
486	84782	brookside	1
487	84782	central	2
488	84782	enterprise	3
489	84782	veyo	0
490	84783	dammeron valley	0
491	84783	veyo	1
492	84784	hildale	0
493	84790	st george	0
494	84791	st george	0
\.


--
-- Data for Name: zip_corrections; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.zip_corrections (id, place, zip, zip_9) FROM stdin;
1	boulder	84716	847161313
2	boulder	84716	847161314
3	boulder	84716	847161315
4	boulder	84716	847161316
5	boulder	84716	847161317
6	boulder	84716	847161318
7	boulder	84716	847161319
8	boulder	84716	847161320
9	boulder	84716	847161321
10	boulder	84716	847161322
11	boulder	84716	847161323
12	boulder	84716	847161324
13	boulder	84716	847161325
14	boulder	84716	847161326
15	boulder	84716	847161327
16	boulder	84716	847161328
17	boulder	84716	847161329
18	boulder	84716	847161330
19	boulder	84716	847161331
20	boulder	84716	847161332
21	boulder	84716	847161333
22	boulder	84716	847161334
23	boulder	84716	847161335
24	boulder	84716	847161336
25	boulder	84716	847161337
26	boulder	84716	847161338
27	boulder	84716	847161339
28	boulder	84716	847161340
29	boulder	84716	847161341
30	boulder	84716	847161342
31	boulder	84716	847161343
32	boulder	84716	847161344
33	boulder	84716	847161345
34	boulder	84716	847161346
35	boulder	84716	847161347
36	boulder	84716	847161348
37	boulder	84716	847161349
38	boulder	84716	847161350
39	boulder	84716	847161351
40	boulder	84716	847161352
41	boulder	84716	847161353
42	boulder	84716	847161354
43	boulder	84716	847161355
44	boulder	84716	847161356
45	boulder	84716	847161357
46	boulder	84716	847161358
47	boulder	84716	847161359
48	boulder	84716	847161360
49	boulder	84716	847161361
50	boulder	84716	847161362
51	boulder	84716	847161363
52	boulder	84716	847161364
53	boulder	84716	847161365
54	boulder	84716	847161366
55	boulder	84716	847161367
56	boulder	84716	847161368
57	boulder	84716	847161369
58	boulder	84716	847161370
59	boulder	84716	847161371
60	boulder	84716	847161372
61	boulder	84716	847161373
62	boulder	84716	847161374
63	boulder	84716	847161375
64	boulder	84716	847161376
65	boulder	84716	847161377
66	boulder	84716	847161378
67	boulder	84716	847161379
68	boulder	84716	847161380
69	boulder	84716	847161381
70	boulder	84716	847161382
71	boulder	84716	847161383
72	boulder	84716	847161384
73	boulder	84716	847161385
74	boulder	84716	847161386
75	boulder	84716	847161387
76	boulder	84716	847161388
77	boulder	84716	847161389
78	boulder	84716	847161390
79	boulder	84716	847161391
80	boulder	84716	847161392
81	boulder	84716	847161393
82	boulder	84716	847161394
83	boulder	84716	847161395
84	boulder	84716	847161396
85	boulder	84716	847161397
86	boulder	84716	847161398
87	boulder	84716	847161399
88	boulder	84716	847161400
89	boulder	84716	847161401
90	boulder	84716	847161402
91	boulder	84716	847161403
92	boulder	84716	847161404
93	boulder	84716	847161405
94	boulder	84716	847161406
95	boulder	84716	847161407
96	boulder	84716	847161408
97	boulder	84716	847161409
98	boulder	84716	847161410
99	boulder	84716	847161411
100	boulder	84716	847161412
101	boulder	84716	847161413
102	boulder	84716	847161414
103	boulder	84716	847161415
104	boulder	84716	847161416
105	boulder	84716	847161417
106	boulder	84716	847161418
107	boulder	84716	847161419
108	boulder	84716	847161420
109	boulder	84716	847161421
110	boulder	84716	847161422
111	boulder	84716	847161423
112	boulder	84716	847161424
113	boulder	84716	847161425
114	boulder	84716	847161426
115	boulder	84716	847161427
116	boulder	84716	847161428
117	boulder	84716	847161429
118	boulder	84716	847161430
119	boulder	84716	847161431
120	boulder	84716	847161432
121	boulder	84716	847161433
122	boulder	84716	847161434
123	boulder	84716	847161435
124	boulder	84716	847161436
125	boulder	84716	847161437
126	boulder	84716	847161438
127	boulder	84716	847161439
128	boulder	84716	847161440
129	boulder	84716	847161441
130	boulder	84716	847161442
131	boulder	84716	847161443
132	boulder	84716	847161444
133	boulder	84716	847161445
134	boulder	84716	847161446
135	boulder	84716	847161447
136	boulder	84716	847161448
137	boulder	84716	847161449
138	boulder	84716	847161450
139	boulder	84716	847161451
140	boulder	84716	847161452
141	boulder	84716	847161453
142	boulder	84716	847161454
143	boulder	84716	847161455
144	boulder	84716	847161456
145	boulder	84716	847161461
146	boulder	84716	847161462
147	boulder	84716	847161463
148	boulder	84716	847161464
149	boulder	84716	847161465
150	boulder	84716	847161466
151	boulder	84716	847161467
152	boulder	84716	847161468
153	boulder	84716	847161469
154	boulder	84716	847161470
155	boulder	84716	847161471
156	boulder	84716	847161472
157	boulder	84716	847161473
158	boulder	84716	847161474
159	boulder	84716	847161475
160	boulder	84716	847161476
161	boulder	84716	847161477
162	boulder	84716	847161478
163	boulder	84716	847161479
164	boulder	84716	847161480
165	boulder	84716	847161481
166	boulder	84716	847161482
167	boulder	84716	847161483
168	boulder	84716	847161484
169	boulder	84716	847161485
170	boulder	84716	847161486
171	boulder	84716	847161487
172	boulder	84716	847161488
173	boulder	84716	847161489
174	boulder	84716	847161490
175	boulder	84716	847161491
176	boulder	84716	847161492
177	boulder	84716	847161493
178	boulder	84716	847161494
179	boulder	84716	847161495
180	boulder	84716	847161496
181	boulder	84716	847161497
182	boulder	84716	847161498
183	boulder	84716	847161499
184	boulder	84716	847161500
185	boulder	84716	847161501
186	boulder	84716	847161502
187	boulder	84716	847161503
188	boulder	84716	847161504
189	boulder	84716	847161505
190	boulder	84716	847161506
191	boulder	84716	847161507
192	boulder	84716	847161508
193	boulder	84716	847161509
194	boulder	84716	847161510
195	boulder	84716	847161511
196	boulder	84716	847161512
197	boulder	84716	847161513
198	boulder	84716	847161514
199	boulder	84716	847161515
200	boulder	84716	847161516
201	boulder	84716	847161517
202	boulder	84716	847161518
203	boulder	84716	847161519
204	boulder	84716	847161520
205	boulder	84716	847169998
206	alta	84092	840928000
207	alta	84092	840928001
208	alta	84092	840928002
209	alta	84092	840928003
210	alta	84092	840928004
211	alta	84092	840928005
212	alta	84092	840928006
213	alta	84092	840928007
214	alta	84092	840928008
215	alta	84092	840928009
216	alta	84092	840928010
217	alta	84092	840928011
218	alta	84092	840928012
219	alta	84092	840928013
220	alta	84092	840928014
221	alta	84092	840928015
222	alta	84092	840928016
223	alta	84092	840928017
224	alta	84092	840928018
225	alta	84092	840928019
226	alta	84092	840928020
227	alta	84092	840928021
228	alta	84092	840928022
229	alta	84092	840928023
230	alta	84092	840928024
231	alta	84092	840928025
232	alta	84092	840928026
233	alta	84092	840928027
234	alta	84092	840928028
235	alta	84092	840928029
236	alta	84092	840928030
237	alta	84092	840928031
238	alta	84092	840928032
239	alta	84092	840928033
240	alta	84092	840928034
241	alta	84092	840928035
242	alta	84092	840928036
243	alta	84092	840928037
244	alta	84092	840928038
245	alta	84092	840928039
246	alta	84092	840928040
247	alta	84092	840928041
248	alta	84092	840928042
249	alta	84092	840928043
250	alta	84092	840928044
251	alta	84092	840928045
252	alta	84092	840928046
253	alta	84092	840928047
254	alta	84092	840928048
255	alta	84092	840928049
256	alta	84092	840928050
257	alta	84092	840928051
258	alta	84092	840928052
259	alta	84092	840928053
260	alta	84092	840928054
261	alta	84092	840928055
262	alta	84092	840928056
263	alta	84092	840928057
264	alta	84092	840928058
265	alta	84092	840928059
266	alta	84092	840928060
267	alta	84092	840928061
268	alta	84092	840928062
269	alta	84092	840928063
270	alta	84092	840928064
271	alta	84092	840928065
272	alta	84092	840928066
273	alta	84092	840928067
274	alta	84092	840928068
275	alta	84092	840928069
276	alta	84092	840928070
277	alta	84092	840928071
278	alta	84092	840928072
279	alta	84092	840928073
280	alta	84092	840928074
281	alta	84092	840928075
282	alta	84092	840928076
283	alta	84092	840928077
284	alta	84092	840928078
285	alta	84092	840928079
286	alta	84092	840928080
287	alta	84092	840928081
288	alta	84092	840928082
289	alta	84092	840928083
290	alta	84092	840928084
291	alta	84092	840928085
292	alta	84092	840928086
293	alta	84092	840928087
294	alta	84092	840928088
295	alta	84092	840928089
296	alta	84092	840928090
297	alta	84092	840928091
298	alta	84092	840928092
299	alta	84092	840928093
300	alta	84092	840928094
301	alta	84092	840928095
302	alta	84092	840928096
303	alta	84092	840928097
304	alta	84092	840928098
305	alta	84092	840928099
306	alta	84092	840928100
307	alta	84092	840928101
308	alta	84092	840928102
309	alta	84092	840928103
310	alta	84092	840928104
311	alta	84092	840928105
312	alta	84092	840928106
313	alta	84092	840928107
314	alta	84092	840928108
315	alta	84092	840928109
316	alta	84092	840928110
317	alta	84092	840928111
318	alta	84092	840928112
319	alta	84092	840928113
320	alta	84092	840928114
321	alta	84092	840928115
322	alta	84092	840928116
323	alta	84092	840928117
324	alta	84092	840928118
325	alta	84092	840928119
326	alta	84092	840928120
327	alta	84092	840928121
328	alta	84092	840928122
329	alta	84092	840928123
330	alta	84092	840928124
331	alta	84092	840928125
332	alta	84092	840928126
333	alta	84092	840928127
334	alta	84092	840928128
335	alta	84092	840928129
336	alta	84092	840928130
337	alta	84092	840928131
338	alta	84092	840928132
339	alta	84092	840928133
340	alta	84092	840928134
341	alta	84092	840928135
342	alta	84092	840928136
343	alta	84092	840928137
344	alta	84092	840928138
345	alta	84092	840928139
346	alta	84092	840928140
347	alta	84092	840928141
348	alta	84092	840928142
349	boulder	84716	847161302
350	boulder	84716	847161303
351	boulder	84716	847161304
352	boulder	84716	847161305
353	boulder	84716	847161306
354	boulder	84716	847161307
355	boulder	84716	847161308
356	boulder	84716	847161309
357	boulder	84716	847161310
358	boulder	84716	847161311
359	boulder	84716	847161312
360	bryce canyon	84764	847640001
361	big water	84741	847412001
362	big water	84741	847412002
363	big water	84741	847412003
364	big water	84741	847412004
365	big water	84741	847412005
366	big water	84741	847412006
367	big water	84741	847412007
368	big water	84741	847412008
369	big water	84741	847412009
370	big water	84741	847412010
371	big water	84741	847412011
372	big water	84741	847412012
373	big water	84741	847412013
374	big water	84741	847412014
375	big water	84741	847412015
376	big water	84741	847412016
377	big water	84741	847412017
378	big water	84741	847412018
379	big water	84741	847412019
380	big water	84741	847412020
381	big water	84741	847412021
382	big water	84741	847412022
383	big water	84741	847412023
384	big water	84741	847412024
385	big water	84741	847412025
386	big water	84741	847412026
387	big water	84741	847412027
388	big water	84741	847412028
389	big water	84741	847412029
390	big water	84741	847412030
391	big water	84741	847412031
392	big water	84741	847412032
393	big water	84741	847412033
394	big water	84741	847412034
395	big water	84741	847412035
396	big water	84741	847412036
397	big water	84741	847412037
398	big water	84741	847412038
399	big water	84741	847412039
400	big water	84741	847412040
401	big water	84741	847412041
402	big water	84741	847412042
403	big water	84741	847412043
404	big water	84741	847412044
405	big water	84741	847412045
406	big water	84741	847412046
407	big water	84741	847412047
408	big water	84741	847412048
409	big water	84741	847412049
410	big water	84741	847412050
411	big water	84741	847412051
412	big water	84741	847412052
413	big water	84741	847412053
414	big water	84741	847412054
415	big water	84741	847412055
416	big water	84741	847412056
417	big water	84741	847412057
418	big water	84741	847412058
419	big water	84741	847412059
420	big water	84741	847412060
421	big water	84741	847412061
422	big water	84741	847412062
423	big water	84741	847412063
424	big water	84741	847412064
425	big water	84741	847412065
426	big water	84741	847412066
427	big water	84741	847412067
428	big water	84741	847412068
429	big water	84741	847412069
430	big water	84741	847412070
431	big water	84741	847412071
432	big water	84741	847412072
433	big water	84741	847412073
434	big water	84741	847412074
435	big water	84741	847412075
436	big water	84741	847412076
437	big water	84741	847412077
438	big water	84741	847412078
439	big water	84741	847412079
440	big water	84741	847412080
441	big water	84741	847412081
442	big water	84741	847412082
443	big water	84741	847412083
444	big water	84741	847412084
445	big water	84741	847412085
446	big water	84741	847412086
447	big water	84741	847412087
448	big water	84741	847412088
449	big water	84741	847412089
450	big water	84741	847412090
451	big water	84741	847412091
452	big water	84741	847412092
453	big water	84741	847412093
454	big water	84741	847412094
455	big water	84741	847412095
456	big water	84741	847412096
457	big water	84741	847412097
458	big water	84741	847412098
459	big water	84741	847412099
460	big water	84741	847412100
461	big water	84741	847412101
462	big water	84741	847412102
463	big water	84741	847412103
464	big water	84741	847412104
465	big water	84741	847412105
466	big water	84741	847412106
467	big water	84741	847412107
468	big water	84741	847412108
469	big water	84741	847412109
470	big water	84741	847412110
471	big water	84741	847412111
472	big water	84741	847412112
473	big water	84741	847412113
474	big water	84741	847412114
475	big water	84741	847412115
476	big water	84741	847412116
477	big water	84741	847412117
478	big water	84741	847412118
479	big water	84741	847412119
480	big water	84741	847412120
481	big water	84741	847412121
482	big water	84741	847412122
483	big water	84741	847412123
484	big water	84741	847412124
485	big water	84741	847412125
486	big water	84741	847412126
487	big water	84741	847412127
488	big water	84741	847412128
489	big water	84741	847412129
490	big water	84741	847412130
491	big water	84741	847412131
492	big water	84741	847412132
493	big water	84741	847412133
494	big water	84741	847412134
495	big water	84741	847412141
496	big water	84741	847412142
497	big water	84741	847412143
498	big water	84741	847412144
499	big water	84741	847412151
500	big water	84741	847412152
501	big water	84741	847412153
502	big water	84741	847412154
503	big water	84741	847412155
504	big water	84741	847412156
505	big water	84741	847412157
506	big water	84741	847412158
507	big water	84741	847412159
508	big water	84741	847412160
509	big water	84741	847412161
510	big water	84741	847412162
511	big water	84741	847412163
512	big water	84741	847412164
513	big water	84741	847412171
514	big water	84741	847412172
515	big water	84741	847412173
516	big water	84741	847412174
517	big water	84741	847412181
518	big water	84741	847412182
519	big water	84741	847412183
520	big water	84741	847412184
521	big water	84741	847412185
522	big water	84741	847412186
523	big water	84741	847412187
524	big water	84741	847412188
525	big water	84741	847412189
526	big water	84741	847412190
527	big water	84741	847412191
528	big water	84741	847412192
529	big water	84741	847412193
530	big water	84741	847412194
531	big water	84741	847412195
532	big water	84741	847412196
533	big water	84741	847412197
534	big water	84741	847412198
535	big water	84741	847412199
536	big water	84741	847412200
537	big water	84741	847412201
538	big water	84741	847412202
539	big water	84741	847412203
540	big water	84741	847412204
541	big water	84741	847412205
542	big water	84741	847412206
543	big water	84741	847412207
544	big water	84741	847412208
545	big water	84741	847412209
546	big water	84741	847412210
547	big water	84741	847412211
548	big water	84741	847412212
549	big water	84741	847412213
550	big water	84741	847412214
551	big water	84741	847412215
552	big water	84741	847412216
553	big water	84741	847412217
554	big water	84741	847412218
555	big water	84741	847412219
556	big water	84741	847412220
557	big water	84741	847412221
558	big water	84741	847412222
559	big water	84741	847412223
560	big water	84741	847412224
561	big water	84741	847412225
562	big water	84741	847412226
563	big water	84741	847412227
564	big water	84741	847412228
565	big water	84741	847412229
566	big water	84741	847412230
567	big water	84741	847412231
568	big water	84741	847412232
569	big water	84741	847412233
570	big water	84741	847412234
571	big water	84741	847412235
572	big water	84741	847412236
573	big water	84741	847412237
574	big water	84741	847412238
575	big water	84741	847412239
576	big water	84741	847412240
577	big water	84741	847412241
578	big water	84741	847412242
579	big water	84741	847412243
580	big water	84741	847412244
581	big water	84741	847412245
582	big water	84741	847412246
583	big water	84741	847412247
584	big water	84741	847412248
585	big water	84741	847412249
586	big water	84741	847412250
587	big water	84741	847412251
588	big water	84741	847412252
589	big water	84741	847412253
590	big water	84741	847412254
591	big water	84741	847412255
592	big water	84741	847412256
593	big water	84741	847412257
594	big water	84741	847412258
595	big water	84741	847412259
596	big water	84741	847412260
597	big water	84741	847412261
598	big water	84741	847412262
599	big water	84741	847412263
600	big water	84741	847412264
601	big water	84741	847412265
602	big water	84741	847412266
603	big water	84741	847412267
604	big water	84741	847412268
605	big water	84741	847412269
606	big water	84741	847412270
607	big water	84741	847412271
608	big water	84741	847412272
609	big water	84741	847412273
610	big water	84741	847412274
611	big water	84741	847412275
612	big water	84741	847412276
613	big water	84741	847412277
614	big water	84741	847412278
615	big water	84741	847412279
616	big water	84741	847412280
617	big water	84741	847412281
618	big water	84741	847412282
619	big water	84741	847412283
620	big water	84741	847412284
621	big water	84741	847412285
622	big water	84741	847412286
623	big water	84741	847412291
624	big water	84741	847412292
625	big water	84741	847412301
626	big water	84741	847412302
627	big water	84741	847412303
628	big water	84741	847412311
629	big water	84741	847412312
630	big water	84741	847412313
631	big water	84741	847412314
632	big water	84741	847412315
633	big water	84741	847412316
634	big water	84741	847412321
635	big water	84741	847412322
636	big water	84741	847412331
637	big water	84741	847412332
638	big water	84741	847412333
639	big water	84741	847412334
640	big water	84741	847412336
641	big water	84741	847412337
642	big water	84741	847412338
643	big water	84741	847412339
644	big water	84741	847412340
645	big water	84741	847412341
646	big water	84741	847412342
647	big water	84741	847412343
648	big water	84741	847412344
649	big water	84741	847412345
650	big water	84741	847412346
651	big water	84741	847412347
652	big water	84741	847412348
653	big water	84741	847412349
654	big water	84741	847412350
655	big water	84741	847412351
656	big water	84741	847412352
657	big water	84741	847412353
658	big water	84741	847412354
659	big water	84741	847412355
660	big water	84741	847412356
661	big water	84741	847412357
662	big water	84741	847412358
663	big water	84741	847412359
664	big water	84741	847412360
665	big water	84741	847412361
666	big water	84741	847412362
667	big water	84741	847412363
668	big water	84741	847412364
669	big water	84741	847412365
670	big water	84741	847412366
671	big water	84741	847412367
672	big water	84741	847412368
673	big water	84741	847412369
674	big water	84741	847412370
675	big water	84741	847412371
676	big water	84741	847412372
677	big water	84741	847412373
678	big water	84741	847412374
679	big water	84741	847412375
680	big water	84741	847412376
681	big water	84741	847412377
682	big water	84741	847412378
683	big water	84741	847412379
684	big water	84741	847412380
685	big water	84741	847412381
686	big water	84741	847412382
687	big water	84741	847412383
688	big water	84741	847412384
689	big water	84741	847412385
690	big water	84741	847412386
691	big water	84741	847412387
692	big water	84741	847412388
693	big water	84741	847412389
694	big water	84741	847412390
695	big water	84741	847412391
696	big water	84741	847412392
697	big water	84741	847412393
698	big water	84741	847412394
699	big water	84741	847412395
700	big water	84741	847412396
701	bryce canyon	84764	847640002
702	bryce canyon	84764	847640003
703	bryce canyon	84764	847640004
704	bryce canyon	84764	847640005
705	bryce canyon	84764	847640006
706	bryce canyon	84764	847640007
707	bryce canyon	84764	847640008
708	bryce canyon	84764	847640009
709	bryce canyon	84764	847640010
710	bryce canyon	84764	847640011
711	bryce canyon	84764	847640012
712	bryce canyon	84764	847640013
713	bryce canyon	84764	847640014
714	bryce canyon	84764	847640015
715	bryce canyon	84764	847640016
716	bryce canyon	84764	847640017
717	bryce canyon	84764	847640018
718	bryce canyon	84764	847640019
719	bryce canyon	84764	847640020
720	bryce canyon	84764	847640021
721	bryce canyon	84764	847640025
722	bryce canyon	84764	847640026
723	bryce canyon	84764	847640027
724	bryce canyon	84764	847640028
725	bryce canyon	84764	847640029
726	bryce canyon	84764	847640030
727	bryce canyon	84764	847640031
728	bryce canyon	84764	847640032
729	bryce canyon	84764	847640033
730	bryce canyon	84764	847640034
731	bryce canyon	84764	847640035
732	bryce canyon	84764	847640036
733	bryce canyon	84764	847640037
734	bryce canyon	84764	847640038
735	bryce canyon	84764	847640039
736	bryce canyon	84764	847640040
737	bryce canyon	84764	847640041
738	bryce canyon	84764	847640042
739	bryce canyon	84764	847640043
740	bryce canyon	84764	847640044
741	bryce canyon	84764	847640045
742	bryce canyon	84764	847640046
743	bryce canyon	84764	847640047
744	bryce canyon	84764	847640048
745	bryce canyon	84764	847640049
746	bryce canyon	84764	847640050
747	bryce canyon	84764	847640051
748	bryce canyon	84764	847640052
749	bryce canyon	84764	847640053
750	bryce canyon	84764	847640054
751	bryce canyon	84764	847640055
752	bryce canyon	84764	847640056
753	bryce canyon	84764	847640057
754	bryce canyon	84764	847640058
755	bryce canyon	84764	847640059
756	bryce canyon	84764	847640060
757	bryce canyon	84764	847640061
758	bryce canyon	84764	847640062
759	bryce canyon	84764	847640063
760	bryce canyon	84764	847640064
761	bryce canyon	84764	847640065
762	bryce canyon	84764	847640067
763	bryce canyon	84764	847640068
764	bryce canyon	84764	847640069
765	bryce canyon	84764	847640070
766	bryce canyon	84764	847640071
767	bryce canyon	84764	847640072
768	bryce canyon	84764	847640073
769	bryce canyon	84764	847640074
770	bryce canyon	84764	847640075
771	bryce canyon	84764	847640076
772	bryce canyon	84764	847640077
773	bryce canyon	84764	847640078
774	bryce canyon	84764	847640079
775	bryce canyon	84764	847640080
776	bryce canyon	84764	847640081
777	bryce canyon	84764	847640082
778	bryce canyon	84764	847640083
779	bryce canyon	84764	847640084
780	bryce canyon	84764	847640085
781	bryce canyon	84764	847640086
782	bryce canyon	84764	847640087
783	bryce canyon	84764	847640088
784	bryce canyon	84764	847640089
785	bryce canyon	84764	847640090
786	bryce canyon	84764	847640100
787	bryce canyon	84764	847640101
788	bryce canyon	84764	847640102
789	bryce canyon	84764	847640103
790	bryce canyon	84764	847640104
791	bryce canyon	84764	847640105
792	bryce canyon	84764	847640106
793	bryce canyon	84764	847640107
794	bryce canyon	84764	847640108
795	bryce canyon	84764	847640109
796	bryce canyon	84764	847640110
797	bryce canyon	84764	847640111
798	bryce canyon	84764	847640112
799	bryce canyon	84764	847640113
800	bryce canyon	84764	847640114
801	bryce canyon	84764	847640115
802	bryce canyon	84764	847640116
803	bryce canyon	84764	847640117
804	bryce canyon	84764	847640118
805	bryce canyon	84764	847640119
806	bryce canyon	84764	847640120
807	bryce canyon	84764	847640121
808	bryce canyon	84764	847640122
809	bryce canyon	84764	847640123
810	bryce canyon	84764	847640124
811	bryce canyon	84764	847640201
812	bryce canyon	84764	847640202
813	bryce canyon	84764	847640203
814	bryce canyon	84764	847640204
815	bryce canyon	84764	847640205
816	bryce canyon	84764	847640206
817	bryce canyon	84764	847640207
818	bryce canyon	84764	847640208
819	bryce canyon	84764	847640209
820	bryce canyon	84764	847640210
821	bryce canyon	84764	847640211
822	bryce canyon	84764	847640212
823	bryce canyon	84764	847640213
824	bryce canyon	84764	847640214
825	bryce canyon	84764	847640215
826	bryce canyon	84764	847640216
827	bryce canyon	84764	847640217
828	bryce canyon	84764	847640218
829	bryce canyon	84764	847640219
830	bryce canyon	84764	847640220
831	bryce canyon	84764	847640221
832	bryce canyon	84764	847640222
833	bryce canyon	84764	847640223
834	bryce canyon	84764	847640224
835	bryce canyon	84764	847640225
836	bryce canyon	84764	847640226
837	bryce canyon	84764	847640227
838	bryce canyon	84764	847640228
839	bryce canyon	84764	847640229
840	bryce canyon	84764	847640230
841	bryce canyon	84764	847640231
842	bryce canyon	84764	847640232
843	bryce canyon	84764	847640233
844	bryce canyon	84764	847640234
845	bryce canyon	84764	847640235
846	bryce canyon	84764	847640236
847	bryce canyon	84764	847640237
848	bryce canyon	84764	847640238
849	bryce canyon	84764	847640239
850	bryce canyon	84764	847640240
851	bryce canyon	84764	847640241
852	bryce canyon	84764	847640242
853	bryce canyon	84764	847640243
854	bryce canyon	84764	847640244
855	bryce canyon	84764	847640245
856	bryce canyon	84764	847640246
857	bryce canyon	84764	847640247
858	bryce canyon	84764	847640248
859	bryce canyon	84764	847640249
860	bryce canyon	84764	847640250
861	bryce canyon	84764	847640251
862	bryce canyon	84764	847640252
863	bryce canyon	84764	847640253
864	bryce canyon	84764	847640254
865	bryce canyon	84764	847640255
866	bryce canyon	84764	847640256
867	bryce canyon	84764	847640257
868	bryce canyon	84764	847640258
869	bryce canyon	84764	847640259
870	bryce canyon	84764	847640260
871	bryce canyon	84764	847640261
872	bryce canyon	84764	847640262
873	bryce canyon	84764	847640263
874	bryce canyon	84764	847640264
875	bryce canyon	84764	847640265
876	bryce canyon	84764	847640266
877	bryce canyon	84764	847640267
878	bryce canyon	84764	847640268
879	bryce canyon	84764	847640269
880	bryce canyon	84764	847640270
881	bryce canyon	84764	847640271
882	bryce canyon	84764	847640272
883	bryce canyon	84764	847640273
884	bryce canyon	84764	847640274
885	bryce canyon	84764	847640275
886	bryce canyon	84764	847640276
887	bryce canyon	84764	847640277
888	bryce canyon	84764	847640278
889	bryce canyon	84764	847640279
890	bryce canyon	84764	847640280
891	bryce canyon	84764	847640281
892	bryce canyon	84764	847640282
893	bryce canyon	84764	847640283
894	bryce canyon	84764	847640284
895	bryce canyon	84764	847640285
896	bryce canyon	84764	847640286
897	bryce canyon	84764	847640287
898	bryce canyon	84764	847640288
899	bryce canyon	84764	847640289
900	bryce canyon	84764	847640290
901	bryce canyon	84764	847640291
902	bryce canyon	84764	847640292
903	bryce canyon	84764	847640293
904	bryce canyon	84764	847640294
905	bryce canyon	84764	847640295
906	bryce canyon	84764	847640296
907	bryce canyon	84764	847640297
908	bryce canyon	84764	847640298
909	bryce canyon	84764	847640299
910	bryce canyon	84764	847640300
911	bryce canyon	84764	847640301
912	bryce canyon	84764	847640302
913	bryce canyon	84764	847640303
914	bryce canyon	84764	847640304
915	bryce canyon	84764	847640305
916	bryce canyon	84764	847640306
917	bryce canyon	84764	847640307
918	bryce canyon	84764	847640308
919	bryce canyon	84764	847640309
920	bryce canyon	84764	847640310
921	bryce canyon	84764	847640311
922	bryce canyon	84764	847640312
923	bryce canyon	84764	847640313
924	bryce canyon	84764	847640314
925	bryce canyon	84764	847640315
926	bryce canyon	84764	847640316
927	bryce canyon	84764	847640317
928	bryce canyon	84764	847640318
929	bryce canyon	84764	847640319
930	bryce canyon	84764	847640320
931	bryce canyon	84764	847640321
932	bryce canyon	84764	847640322
933	bryce canyon	84764	847640323
934	bryce canyon	84764	847640324
935	bryce canyon	84764	847640325
936	bryce canyon	84764	847640326
937	bryce canyon	84764	847640327
938	bryce canyon	84764	847640328
939	bryce canyon	84764	847640329
940	bryce canyon	84764	847640330
941	bryce canyon	84764	847640331
942	bryce canyon	84764	847640332
943	bryce canyon	84764	847640333
944	bryce canyon	84764	847640334
945	bryce canyon	84764	847640335
946	bryce canyon	84764	847640336
947	bryce canyon	84764	847640337
948	bryce canyon	84764	847640338
949	bryce canyon	84764	847640339
950	bryce canyon	84764	847640340
951	bryce canyon	84764	847640341
952	bryce canyon	84764	847640342
953	bryce canyon	84764	847640343
954	bryce canyon	84764	847640344
955	bryce canyon	84764	847640345
956	bryce canyon	84764	847640346
957	bryce canyon	84764	847640347
958	bryce canyon	84764	847640348
959	bryce canyon	84764	847640349
960	bryce canyon	84764	847640350
961	bryce canyon	84764	847640351
962	bryce canyon	84764	847640352
963	bryce canyon	84764	847640353
964	bryce canyon	84764	847640354
965	bryce canyon	84764	847640355
966	bryce canyon	84764	847640356
967	bryce canyon	84764	847640357
968	bryce canyon	84764	847640358
969	bryce canyon	84764	847640359
970	bryce canyon	84764	847640360
971	bryce canyon	84764	847640361
972	bryce canyon	84764	847640362
973	bryce canyon	84764	847640363
974	bryce canyon	84764	847640364
975	bryce canyon	84764	847640365
\.


--
-- Name: Placenames_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Placenames_id_seq"', 829, true);


--
-- Name: accounts_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.accounts_id_seq', 10, true);


--
-- Name: apikeys_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.apikeys_id_seq', 1, false);


--
-- Name: delivery_points_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.delivery_points_id_seq', 17, true);


--
-- Name: poboxes_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.poboxes_id_seq', 286, true);


--
-- Name: zip_corrections_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.zip_corrections_id_seq', 975, true);


--
-- Name: zipcodes_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.zipcodes_id_seq', 484, true);


--
-- Name: place_names Placenames_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.place_names
    ADD CONSTRAINT "Placenames_pkey" PRIMARY KEY (id);


--
-- Name: accounts accounts_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accounts
    ADD CONSTRAINT accounts_pkey PRIMARY KEY (id);


--
-- Name: apikeys apikeys_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.apikeys
    ADD CONSTRAINT apikeys_pkey PRIMARY KEY (id);


--
-- Name: delivery_points delivery_points_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.delivery_points
    ADD CONSTRAINT delivery_points_pkey PRIMARY KEY (id);


--
-- Name: po_boxes poboxes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.po_boxes
    ADD CONSTRAINT poboxes_pkey PRIMARY KEY (id);


--
-- Name: zip_corrections zip_corrections_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.zip_corrections
    ADD CONSTRAINT zip_corrections_pkey PRIMARY KEY (id);


--
-- Name: zip_codes zipcodes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.zip_codes
    ADD CONSTRAINT zipcodes_pkey PRIMARY KEY (id);


--
-- Name: apikeys apikeys_account_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.apikeys
    ADD CONSTRAINT apikeys_account_id_fkey FOREIGN KEY (account_id) REFERENCES public.accounts(id) ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

--
-- PostgreSQL database cluster dump complete
--
