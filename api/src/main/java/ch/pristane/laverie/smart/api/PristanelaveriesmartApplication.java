package ch.pristane.laverie.smart.api;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.persistence.autoconfigure.EntityScan;
import org.springframework.data.jpa.repository.config.EnableJpaRepositories;

@SpringBootApplication(scanBasePackages = "ch.pristane.laverie.smart")
@EnableJpaRepositories(basePackages = "ch.pristane.laverie.smart.infrastructure.persistence.repositories")
@EntityScan(basePackages = "ch.pristane.laverie.smart.infrastructure.persistence.entities")
public class PristanelaveriesmartApplication {

	public static void main(String[] args) {
		SpringApplication.run(PristanelaveriesmartApplication.class, args);
	}

}
