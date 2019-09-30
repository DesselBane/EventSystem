public delete(hero: Hero): Observable<Hero> {
  return this.httpClient.delete<Hero>(`${this.URL}/${hero._id}`);
}
