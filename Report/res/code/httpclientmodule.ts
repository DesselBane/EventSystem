public delete(hero: Hero): Observable<Hero> {
  return this.http
    .delete(`${this.URL}/${hero._id}`)
    .map(result => hero);
}
