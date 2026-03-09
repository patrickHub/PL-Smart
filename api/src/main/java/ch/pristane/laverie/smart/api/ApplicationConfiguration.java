package ch.pristane.laverie.smart.api;

import com.fasterxml.jackson.databind.ObjectMapper;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;


@Configuration
public class ApplicationConfiguration {

    private static final Logger LOGGER = LoggerFactory.getLogger(ApplicationConfiguration.class);


    @Bean
    public ObjectMapper objectMapper() { 
    return new ObjectMapper();
    }
}